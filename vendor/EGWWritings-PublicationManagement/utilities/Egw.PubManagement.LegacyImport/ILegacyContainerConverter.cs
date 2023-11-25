using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport;

/// <summary>
/// Legacy container-level converter
/// </summary>
public interface ILegacyContainerConverter
{
    /// <summary>
    /// If converter can process node
    /// </summary>
    /// <param name="node"></param>
    bool CanProcess(INode node);

    /// <summary>
    /// Converts node to block element
    /// </summary>
    /// <param name="parser">Element parser</param>
    /// <param name="node">Node</param>
    /// <param name="context"></param>
    /// <returns></returns>
    IWemlContainerElement Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context);
}