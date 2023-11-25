using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class NewPageConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "P" or "DIV" } el && el.ClassName == "page";

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context) =>
        new WemlPageBreakElement(node.TextContent.Trim());
}