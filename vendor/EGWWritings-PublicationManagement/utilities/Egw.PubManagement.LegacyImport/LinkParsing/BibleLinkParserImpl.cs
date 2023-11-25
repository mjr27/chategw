using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.BibleCodes;
using Egw.PubManagement.LegacyImport.LinkRepository;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.LinkParsing;

/// <summary>
/// Link parser default implementation
/// </summary>
public class BibleLinkParserImpl : ILinkParser
{
    private readonly ILinkRepository _linkRepository;
    private readonly BibleLinkNormalizer _normalizer;

    /// <summary>
    /// Default constructor
    /// </summary>
    public BibleLinkParserImpl(ILinkRepository linkRepository)
    {
        _linkRepository = linkRepository;
        _normalizer = new BibleLinkNormalizer();
    }


    /// <inheritdoc />
    public bool TryResolveReference(IElement element,
        ILegacyParserContext context,
        out ParaId reference,
        out bool isBible,
        out string? title)
    {
        reference = default;
        isBible = true;
        title = null;

        string? className = element.ClassName;
        if (string.IsNullOrWhiteSpace(className))
        {
            return false;
        }

        if (!className.StartsWith("bible-"))
        {
            return false;
        }

        string[] a = className.Split('-', 2);
        string lang = a[1];
        string elTitle = element.GetAttribute("title") ?? "";

        if (string.IsNullOrWhiteSpace(elTitle))
        {
            return false;
        }

        title = elTitle.Trim();
        int dashIndex = elTitle.IndexOf('-');
        if (dashIndex > 0)
        {
            elTitle = elTitle[..dashIndex];
        }

        elTitle = _normalizer.NormalizeRefCode(elTitle);

        string? languageCode = _linkRepository.NormalizeLanguage(lang);

        if (languageCode is null)
        {
            return false;
        }

        string? elementId = _linkRepository.FindParagraph($"{a[0]} {elTitle}", languageCode)
                            ?? _linkRepository.FindParagraph($"{a[0]} {elTitle}", "en-us")
                            ?? _linkRepository.FindParagraph(elTitle, languageCode)
                            ?? _linkRepository.FindParagraph(elTitle, "en-us");
        return elementId is not null && ParaId.TryParse(elementId, out reference);
    }
}