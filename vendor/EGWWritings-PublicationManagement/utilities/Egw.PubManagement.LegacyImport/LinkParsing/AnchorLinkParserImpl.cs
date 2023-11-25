using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.LinkRepository;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.LinkParsing;

internal class AnchorLinkParserImpl : ILinkParser
{
    private readonly ILinkRepository _linkRepository;

    public AnchorLinkParserImpl(ILinkRepository linkRepository)
    {
        _linkRepository = linkRepository;
    }

    public bool TryResolveReference(IElement element,
        ILegacyParserContext context,
        out ParaId reference,
        out bool isBible,
        out string? title)
    {
        reference = default;
        isBible = default;
        title = null;

        string className = element.ClassName ?? "egw-eng";

        string[] a = className.Split('-', 2);
        string languageCode = _linkRepository.NormalizeLanguage(a[1]) ?? "en-us";

        string elTitle = element.GetAttribute("href") ?? element.GetAttribute("title") ?? "";

        if (!elTitle.Contains('#')) // no # in title
        {
            return false;
        }

        string[] r = elTitle.Split('#', 2);
        string book = r[0];
        string link = r[1];

        if (string.IsNullOrWhiteSpace(book))
        {
            book = context.Header.Code;
        }

        string totalLink = $"{book}#{link}";
        string? paragraphRef = _linkRepository.FindParagraph(totalLink, languageCode);
        if (paragraphRef is null)
        {
            return false;
        }

        isBible = false;
        return ParaId.TryParse(paragraphRef, out reference);
    }
}