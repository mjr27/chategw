using AngleSharp.Dom;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Container;

internal class ListContainerConverter : ILegacyContainerConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "UL" or "OL" };

    public IWemlContainerElement Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        IWemlBlockNode? list = parser.ParseBlock(node, context);
        if (list is not null)
        {
            return new WemlParagraphContainer(list);
        }

        context.Report(WarningLevel.Error, node, "Cannot parse table");
        list = new WemlTextBlockElement(WemlParagraphType.Paragraph, ArraySegment<IWemlInlineNode>.Empty);
        return new WemlParagraphContainer(list);
    }
}