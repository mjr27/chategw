using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.BlockChildren;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Block;

internal class ListConverter : ILegacyBlockConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "UL" or "OL" };

    public IWemlBlockNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        WemlListType? listType = node.NodeName switch
        {
            "UL" => WemlListType.Unordered,
            "OL" => WemlListType.Ordered,
            _ => null
        };
        
        if (listType is null)
        {
            context.Report(WarningLevel.Error, node, "Unknown list type");
            listType = WemlListType.Unordered;
        }

        var result = new WemlListElement(listType.Value, ArraySegment<WemlListItemNode>.Empty);
        foreach (IElement child in element.Children)
        {
            if (child.NodeName != "LI")
            {
                context.Report(WarningLevel.Error, child, "List child element is not <li>");
                continue;
            }

            result.Children.Add(new WemlListItemNode(parser.ParseChildBlocks(child, context)));
        }

        return result;
    }
}