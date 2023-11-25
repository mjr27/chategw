using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport;

/// <summary>
/// element parser
/// </summary>
public interface ILegacyElementParser
{
    /// <summary>
    /// Parses inline using one of converters
    /// </summary>
    /// <param name="node">Node</param>
    /// <param name="context"></param>
    /// <returns></returns>
    IWemlInlineNode? ParseInline(INode node, ILegacyParserContext context);

    /// <summary>
    /// Parses block using one of converters
    /// </summary>
    /// <param name="node">Node</param>
    /// <param name="context"></param>
    /// <returns></returns>
    IWemlBlockNode? ParseBlock(INode node, ILegacyParserContext context);

    /// <summary>
    /// Parses container using one of converters
    /// </summary>
    /// <param name="node">Node</param>
    /// <param name="context">Context</param>
    /// <returns></returns>
    IWemlContainerElement? ParseContainer(INode node, ILegacyParserContext context);
}