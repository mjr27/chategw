using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport;

/// <summary>
/// Legacy inline-level converter
/// </summary>
public interface ILegacyInlineConverter
{
    /// <summary>
    /// If converter can process node
    /// </summary>
    /// <param name="node"></param>
    bool CanProcess(INode node);

    /// <summary>
    /// Converts node to inline element
    /// </summary>
    /// <param name="parser">Element parser</param>
    /// <param name="node">Node</param>
    /// <param name="context"></param>
    /// <returns></returns>
    IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context);
}