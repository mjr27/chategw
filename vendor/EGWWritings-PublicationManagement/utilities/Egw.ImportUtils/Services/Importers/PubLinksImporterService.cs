using Dapper;

using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.ImportUtils.Services.Importers;

public class PubLinksImporterService : ImporterServiceBase
{
    private record BibleContentDto(int ElementId, string RefCode1, string RefCode2, string RefCode3, string RefCode4);

    private record TranslationDto(int OriginalElementId, int ElementId);

    private readonly ILogger<PubLinksImporterService> _logger;

    public PubLinksImporterService(
        ILogger<PubLinksImporterService> logger,
        string publicationDbConnectionString,
        string egwConnectionString
    ) : base(publicationDbConnectionString, egwConnectionString)
    {
        _logger = logger;
    }

    public override async Task Import(CancellationToken cancellationToken)
    {
        await using IDbContextTransaction t = await _db.Database.BeginTransactionAsync(cancellationToken);
        await _db.PublicationLinks.ExecuteDeleteAsync(cancellationToken: cancellationToken);
        await _db.PublicationLinks.BulkInsertAsync(FetchLinks(), cancellationToken);
        await _db.PublicationLinks.BulkInsertAsync(FetchBibleLinks(1965), cancellationToken);
        await t.CommitAsync(cancellationToken);
    }

    private IEnumerable<ParagraphLink> FetchLinks()
    {
        PublicationType[] availableTypes =
        {
            PublicationType.Book, PublicationType.Devotional, PublicationType.BibleCommentary, PublicationType.PeriodicalPageBreak,
            PublicationType.PeriodicalNoPageBreak
        };
        var availableTranslations = _db.Publications
            .AsNoTracking()
            .Where(p => availableTypes.Contains(p.Type) && p.OriginalPublicationId != null)
            .Select(r => new { r.PublicationId, OriginalPublicationId = r.OriginalPublicationId!.Value })
            .ToDictionary(r => r.PublicationId, r => r.OriginalPublicationId);
        foreach ((int publicationId, int originalPublicationId) in availableTranslations)
        {
            IEnumerable<TranslationDto> translations = _egwDb.Query<TranslationDto>(
                ImportTranslationsQuery, new { originalPublicationId, publicationId });
            foreach (TranslationDto? row in translations)
            {
                yield return new ParagraphLink(
                    new ParaId(publicationId, row.ElementId),
                    new ParaId(originalPublicationId, row.OriginalElementId)
                );
            }
        }
    }

    // ReSharper disable once CognitiveComplexity
    private IEnumerable<ParagraphLink> FetchBibleLinks(int sourceBibleId)
    {
        var bibles = _db.Publications
            .Where(r => r.Type == PublicationType.Bible && r.PublicationId != sourceBibleId)
            .ToList();
        List<string> kjvBooks = GetBibleBooks(sourceBibleId);

        Dictionary<string, List<string>> bibleBookTranslations = FetchBibleBookTranslations(bibles, kjvBooks);

        var kjvData = GetBibleContent(sourceBibleId)
            .ToDictionary(r => (r.RefCode2, r.RefCode3, r.RefCode4), r => r.ElementId);
        int errors = 0;
        foreach (Publication bible in bibles)
        {
            foreach (BibleContentDto row in GetBibleContent(bible.PublicationId))
            {
                if (!bibleBookTranslations.ContainsKey(row.RefCode1) || !bibleBookTranslations[row.RefCode1].Contains(row.RefCode2))
                {
                    _logger.LogWarning(
                        "Error fetching translation for refcode1={Refcode1}; refcode2={Refcode2}",
                        row.RefCode1,
                        row.RefCode2);
                    continue;
                }

                string kjvRefcode2 = kjvBooks[bibleBookTranslations[row.RefCode1].IndexOf(row.RefCode2)];
                if (!kjvData.TryGetValue((kjvRefcode2, row.RefCode3, row.RefCode4), out int srcElementId))
                {
                    _logger.LogWarning(
                        "Cannot find kjv element for refcode1={Refcode1}; refcode2={Refcode2}; refcode3={Refcode3}; refcode4={Refcode4} in book {Book}",
                        row.RefCode1,
                        row.RefCode2,
                        row.RefCode3,
                        row.RefCode4,
                        bible.PublicationId);
                    errors++;
                    continue;
                }

                yield return new ParagraphLink(new ParaId(bible.PublicationId, row.ElementId),
                    new ParaId(sourceBibleId, srcElementId)
                );
            }
        }

        _logger.LogWarning("Total errors: {Errors}", errors);
    }

    private Dictionary<string, List<string>> FetchBibleBookTranslations(IEnumerable<Publication> bibles,
        ICollection<string> kjvBooks)
    {
        var bibleBookTranslations = new Dictionary<string, List<string>>();
        foreach (Publication bible in bibles)
        {
            List<string> bibleBooks = GetBibleBooks(bible.PublicationId);
            if (bibleBooks.Count != kjvBooks.Count)
            {
                _logger.LogWarning(
                    "Wrong count of books in {Pubnr} ({Lang}/{Code}) {Title}",
                    bible.PublicationId,
                    bible.LanguageCode,
                    bible.Code,
                    bible.Title);
                continue;
            }

            bibleBookTranslations[bible.Code] = bibleBooks;
        }

        return bibleBookTranslations;
    }

    private List<string> GetBibleBooks(int bibleId)
    {
        return _egwDb.Query<string>(@"-- noinspection SqlDialectInspectionForFile
select p.refcode2
from sqlb_publications p
         inner join sqlb_publicationoverview po on p.id_pub = po.id_pub
where po.actversion = 1
  and po.pubnr = @bibleId
  and p.element in ('h1', 'h2')
  and TRIM(p.refcode2) != ''
order by puborder", new { bibleId }).ToList();
    }

    private IEnumerable<BibleContentDto> GetBibleContent(int bibleId)
    {
        return _egwDb.Query<BibleContentDto>(@"-- noinspection SqlDialectInspectionForFile
select p.id_element as ElementId,
       p.refcode1   as RefCode1,
       p.refcode2   as RefCode2,
       p.refcode3   as RefCode3,
       p.refcode4   as RefCode4

from sqlb_publications p
         inner join sqlb_publicationoverview po on p.id_pub = po.id_pub
where po.actversion = 1
  and po.pubnr = @bibleId
  and p.element != 'h4'
  and TRIM(p.refcode2) != ''
order by puborder", new { bibleId });
    }


    private const string ImportTranslationsQuery = @"-- noinspection SqlDialectInspectionForFile
select distinct id_element_eng  as OriginalElementId,
       id_element_lang as ElementId
from sqlb_publinks
where pubnr_eng = @originalPublicationId 
  and pubnr_lang = @publicationId
  and id_element_eng is not null
  and id_element_lang is not null
  and id_element_eng > 0
  and id_element_lang > 0;
";
}