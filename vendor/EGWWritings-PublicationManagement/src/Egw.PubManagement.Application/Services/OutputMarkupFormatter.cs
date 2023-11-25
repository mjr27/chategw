using AngleSharp.Dom;
using AngleSharp.Html;

namespace Egw.PubManagement.Application.Services;

/// <summary>
/// Outputs HTML with indentation
/// </summary>
public class OutputMarkupFormatter : HtmlMarkupFormatter
{
    /// <inheritdoc />
    public override string Doctype(IDocumentType doctype)
    {
        return base.Doctype(doctype) + "\n";
    }

    /// <inheritdoc />
    public override String OpenTag(IElement element, Boolean selfClosing)
    {
        string result = base.OpenTag(element, selfClosing);
        int? indent = GetLevel(element);

        return indent switch
        {
            0 => result + "\n",
            > 0 => Indent(indent.Value) + result + "\n" + Indent(indent.Value + 1),
            < 0 => Indent(-indent.Value) + result,
            _ => result
        };
    }

    /// <inheritdoc />
    public override String CloseTag(IElement element, Boolean selfClosing)
    {
        string result = base.CloseTag(element, selfClosing);
        int? indent = GetLevel(element);
        return indent switch
        {
            0 => result + "\n",
            > 0 => "\n" + Indent(indent.Value) + result + "\n",
            _ => result
        };
    }

    private const string Indent0 = "";
    private const string Indent1 = "    ";
    private const string Indent2 = "        ";

    private static string Indent(int count)
    {
        return count switch
        {
            1 => Indent1,
            2 => Indent2,
            _ => Indent0
        };
    }

    private int? GetLevel(IElement element)
    {
        return element switch
        {
            { TagName: "HTML" or "HEAD" or "BODY" } => 0,
            { TagName: "META" or "DIV" } => 1,
            { TagName: "TITLE" } => null,
            _ => null
        };
    }
}