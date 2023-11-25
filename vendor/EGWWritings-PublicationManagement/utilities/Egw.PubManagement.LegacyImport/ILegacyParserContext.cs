using AngleSharp.Dom;
using AngleSharp.Html.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport;

/// <summary>
/// Parser context
/// </summary>
public interface ILegacyParserContext
{
    /// <summary>
    /// Document header
    /// </summary>
    public WemlDocumentHeader Header { get; }
    /// <summary>
    /// HTML Document
    /// </summary>
    public IHtmlDocument Document { get; }

    /// <summary>
    /// Warning collector
    /// </summary>
    public void Report(WarningLevel level, INode node, string errorMessage);
}