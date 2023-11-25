using AngleSharp.Dom;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Container;

internal class TableContainerConverter : ILegacyContainerConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "TABLE" };

    public IWemlContainerElement Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        IWemlBlockNode? table = parser.ParseBlock(node, context);

        if (table is not null) // Skip table numbering
        {
            return new WemlParagraphContainer(table);
        }

        context.Report(WarningLevel.Error, node, "Cannot parse table");
        table = new WemlTextBlockElement(WemlParagraphType.Paragraph, ArraySegment<IWemlInlineNode>.Empty);

        return new WemlParagraphContainer(table);
    }
}