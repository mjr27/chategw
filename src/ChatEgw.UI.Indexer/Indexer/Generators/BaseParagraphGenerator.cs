using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using ChatEgw.UI.Persistence;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Services;
using WhiteEstate.DocFormat;

namespace ChatEgw.UI.Indexer.Indexer.Generators;

public abstract partial class BaseParagraphGenerator
{
    protected record ParagraphModel(ParaId ParaId, string Content, ParagraphMetadata? Metadata,
        List<string> References);
    // protected record ParagraphModel(ParaId ParaId, string Content, List<string> References);

    private readonly PublicationDbContext _db;
    protected readonly IndexPublicationDto Publication;
    private readonly IElement _div;
    private readonly IDocument _document;

    protected BaseParagraphGenerator(PublicationDbContext db, IndexPublicationDto publication)
    {
        _db = db;
        Publication = publication;

        using IBrowsingContext context = BrowsingContext.New();
        _document = context.OpenNewAsync().Result;
        _div = _document.CreateElement("div");
    }

    public IEnumerable<SearchParagraph> Index()
    {
        IOrderedQueryable<Paragraph> paragraphs = _db.Paragraphs
            .Where(r => r.PublicationId == Publication.PublicationId)
            .OrderBy(r => r.Order);
        foreach (ParagraphModel paragraph in FetchParagraphs(paragraphs))
        {
            var para = new SearchParagraph
            {
                Id = (long)paragraph.ParaId.PublicationId * 1_000_000_000 + paragraph.ParaId.ElementId,
                Content = ExtractContent(paragraph.Content),
                Uri = new Uri($"https://egw.bz/p/{paragraph.ParaId}"),
                RefCode = IRefCodeGenerator.Instance.Long(
                              Publication.Type,
                              Publication.Title,
                              paragraph.Metadata!
                          )
                          ?? "",
                IsEgw = Publication.IsEgw,
                NodeId = $"p{Publication.PublicationId}",
                References = paragraph.References
                    .Distinct()
                    .Select(reference => new SearchParagraphReference { ReferenceCode = reference }).ToList(),
            };
            if (string.IsNullOrWhiteSpace(para.RefCode)
                || string.IsNullOrWhiteSpace(para.Content)
                || para.Content.Length < 50
                || !para.Content.Any(char.IsLetter)
               )
            {
                continue;
            }

            yield return para;
        }
    }

    protected virtual IEnumerable<string> GetReferences(ParagraphMetadata? paragraphMetadata)
    {
        string[] refCodes = GetReferenceCodes(paragraphMetadata)
            .Select(NormalizeTitle)
            .Distinct()
            .ToArray();
        if (refCodes.Length == 0)
        {
            yield break;
        }

        foreach (string publicationTitle in Publication.PublicationCodes
                     .Append(Publication.Code)
                     .Append(Publication.Title))
        {
            string normalizedTitle = NormalizeTitle(publicationTitle);
            yield return normalizedTitle;
            foreach (string refCode in refCodes.Select(NormalizeTitle))
            {
                yield return $"{normalizedTitle} {refCode}";
            }
        }
    }

    protected static string NormalizeTitle(string s)
    {
        return ReSpaces().Replace(s.ToLowerInvariant(), " ").Normalize();
    }


    protected abstract IEnumerable<string> GetReferenceCodes(ParagraphMetadata? paragraphMetadata);

    protected abstract IEnumerable<ParagraphModel> FetchParagraphs(IQueryable<Paragraph> paragraphs);

    private string ExtractContent(string content)
    {
        content = content.Replace("</w-text-block>", "</w-text-block> ");
        content = ReBr().Replace(content, "\n");
        _div.InnerHtml = content;
        IElement[] notes = _div.QuerySelectorAll("w-note, w-non-egw").ToArray();
        foreach (IElement note in notes)
        {
            note.Replace(_document.CreateTextNode(" "));
        }

        content = _div.TextContent;
        content = ReSpaces().Replace(content, " ").Trim();
        content = RePunctuationBetweenCharacters().Replace(content, " ");
        return content;
    }


    [GeneratedRegex("\\s+")]
    private static partial Regex ReSpaces();

    [GeneratedRegex(@"<br\s*/?>")]
    private static partial Regex ReBr();
    [GeneratedRegex(@"(?<=[\.,;:!?])(?=[\p{L}])")]
    private static partial Regex RePunctuationBetweenCharacters();
}