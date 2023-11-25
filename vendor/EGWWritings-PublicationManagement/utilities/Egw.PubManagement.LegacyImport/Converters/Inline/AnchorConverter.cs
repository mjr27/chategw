using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class AnchorConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "A" } el && el.HasAttribute("name");

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        var childNodes = new List<IWemlInlineNode> { new WemlAnchorNode(element.GetAttribute("name")!) };
        if (node.ChildNodes.Any())
        {
            childNodes.AddRange(parser.ParseChildInlines(node, context));
        }

        return new WemlDummyInlineNode(childNodes);
    }
}