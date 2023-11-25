using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Entities.Metadata;
using WhiteEstate.DocFormat;

namespace ChatEgw.UI.Indexer.Indexer.Generators;

internal sealed class BibleParagraphGenerator : BaseParagraphGenerator
{
    private readonly int _level;

    public BibleParagraphGenerator(PublicationDbContext db, IndexPublicationDto publication, int level) : base(db,
        publication)
    {
        _level = level;
    }

    protected override IEnumerable<string> GetReferenceCodes(ParagraphMetadata? paragraphMetadata)
    {
        if (paragraphMetadata?.BibleMetadata is null)
        {
            yield break;
        }

        BibleMetadata? meta = paragraphMetadata.BibleMetadata;
        yield return meta.Book;
        if (meta.Chapter == 0) yield break;
        yield return $"{meta.Book} {meta.Chapter}";
        foreach (int verse in meta.Verses)
        {
            yield return $"{meta.Book} {meta.Chapter} {verse}";
        }
    }

    protected override IEnumerable<ParagraphModel> FetchParagraphs(IQueryable<Paragraph> paragraphs)
    {
        var currentContent = new ParagraphModel(new ParaId(), "", null);
        foreach (var paragraph in paragraphs.Where(r => r.HeadingLevel == 0 || r.HeadingLevel >= _level)
                     .Select(
                         r => new
                         {
                             r.ParaId,
                             r.Order,
                             r.HeadingLevel,
                             r.Content,
                             r.Metadata
                         }
                     ))
        {
            if (paragraph.HeadingLevel == _level)
            {
                if (!currentContent.ParaId.IsEmpty)
                {
                    yield return currentContent;
                }

                currentContent = new ParagraphModel(
                    paragraph.ParaId,
                    "",
                    paragraph.Metadata
                );
            }
            else
            {
                currentContent = currentContent with { Content = currentContent.Content + " " + paragraph.Content };
            }
        }

        if (!currentContent.ParaId.IsEmpty)
        {
            yield return currentContent;
        }
    }

    protected override IEnumerable<string> GetReferences(ParagraphMetadata? paragraphMetadata)
    {
        string[] refCodes = GetReferenceCodes(paragraphMetadata)
            .Select(NormalizeTitle).ToArray();
        if (refCodes.Length == 0)
        {
            yield break;
        }

        foreach (string refCode in refCodes)
        {
            yield return refCode;
        }

        foreach (string publicationTitle in Publication.PublicationCodes
                     .Append(Publication.Code)
                     .Append(Publication.Title))
        {
            string normalizedTitle = NormalizeTitle(publicationTitle);
            foreach (string refCode in refCodes)
            {
                yield return refCode;
                yield return $"{normalizedTitle} {refCode}";
            }
        }
    }
}