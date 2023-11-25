using AngleSharp.Dom;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Container;

internal class HeadingContainerConverter : ILegacyContainerConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "H1" or "H2" or "H3" or "H4" or "H5" or "H6" };

    public IWemlContainerElement Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var el = (IElement)node;
        IWemlBlockNode? block = parser.ParseBlock(node, context);
        if (block is not WemlTextBlockElement para)
        {
            context.Report(WarningLevel.Error, node, "Unable to parse header");
            return new WemlHeadingContainer(
                1,
                new WemlTextBlockElement(WemlParagraphType.Paragraph, ArraySegment<IWemlInlineNode>.Empty)
            );
        }
        return new WemlHeadingContainer(int.Parse(el.NodeName[1..]), para);
    }
}