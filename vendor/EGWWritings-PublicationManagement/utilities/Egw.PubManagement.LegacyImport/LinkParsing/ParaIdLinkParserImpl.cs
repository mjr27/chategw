using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.LinkParsing;

/// <summary>
/// Link parser default implementation
/// </summary>
public class ParaIdLinkParserImpl : ILinkParser
{
    /// <inheritdoc />
    public bool TryResolveReference(
        IElement element,
        ILegacyParserContext context,
        out ParaId reference,
        out bool isBible,
        out string? title)
    {
        reference = default;
        isBible = false;
        title = null;
        string? elementTitle = element.GetAttribute("title");
        if (string.IsNullOrWhiteSpace(elementTitle))
        {
            return false;
        }

        if (!elementTitle.StartsWith("pubnr_id."))
        {
            return false;
        }

        string[] a = elementTitle.Split('.', 3);
        if (!int.TryParse(a[1], out int bookId))
        {
            return false;
        }

        if (!int.TryParse(a[2], out int elementId))
        {
            return false;
        }

        string? className = element.ClassName;
        isBible = className is not null && className.StartsWith("bible-");
        title = element.GetAttribute("data-title");
        reference = new ParaId(bookId, elementId);
        return true;
    }
}