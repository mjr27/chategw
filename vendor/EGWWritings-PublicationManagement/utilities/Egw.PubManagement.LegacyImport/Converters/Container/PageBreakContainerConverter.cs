using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters.Container;

internal class PageBreakContainerConverter : ILegacyContainerConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "P" or "DIV" } el && el.ClassName == "page";

    public IWemlContainerElement Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
        => new WemlPageBreakElement(node.TextContent.Trim());
}