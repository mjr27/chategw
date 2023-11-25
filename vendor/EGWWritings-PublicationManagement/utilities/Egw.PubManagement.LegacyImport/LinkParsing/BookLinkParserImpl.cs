using System.Text.RegularExpressions;

using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.LinkRepository;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.LinkParsing;

/// <summary>
/// Link parser default implementation
/// </summary>
public class BookLinkParserImpl : ILinkParser
{
    private readonly ILinkRepository _linkRepository;

    /// <summary>
    /// Default constructor
    /// </summary>
    public BookLinkParserImpl(ILinkRepository linkRepository)
    {
        _linkRepository = linkRepository;
    }

    /// <inheritdoc />
    public bool TryResolveReference(IElement element,
        ILegacyParserContext context,
        out ParaId reference,
        out bool isBible,
        out string? title)
    {
        reference = default;
        isBible = false;
        title = null;

        string? className = element.ClassName;
        if (className is null)
        {
            return false;
        }

        if (className.StartsWith("bible"))
        {
            return false;
        }

        string[] a = className.Split('-', 2);
        string lang = a[1];

        string elTitle = element.GetAttribute("title") ?? "";

        if (elTitle.Contains('#')) // no # in title
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(elTitle))
        {
            return false;
        }

        title = elTitle;

        elTitle = CleanRefCode(elTitle);

        string? languageCode = _linkRepository.NormalizeLanguage(lang);

        if (languageCode is null)
        {
            return false;
        }

        elTitle = ReTrailingZero.Replace(elTitle, "");

        string? elementId = _linkRepository.FindParagraph(elTitle, languageCode);

        return elementId is not null && ParaId.TryParse(elementId, out reference);
    }

    private static string CleanRefCode(string refCode)
    {
        refCode = refCode.Normalize().ToLowerInvariant();
        string bookCode;
        string remainder = "";
        if (refCode.Contains(' '))
        {
            string[] arr = refCode.Split(' ', 2);
            bookCode = arr[0];
            remainder = arr[1];
        }
        else
        {
            bookCode = refCode;
        }

        remainder = ReSpaces.Replace(remainder, " ");

        if (bookCode.StartsWith("sg-", StringComparison.OrdinalIgnoreCase))
        {
            bookCode = bookCode[3..] + "-sg";
        }

        return (bookCode + " " + remainder).Trim();
    }

    private static readonly Regex ReSpaces = new(@"[\p{P}\s]+", RegexOptions.Compiled | RegexOptions.Multiline);
    private static readonly Regex ReTrailingZero = new(@"\s+0\s*$", RegexOptions.Compiled | RegexOptions.Multiline);
}