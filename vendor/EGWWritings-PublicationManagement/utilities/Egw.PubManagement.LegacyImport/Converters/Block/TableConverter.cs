using AngleSharp.Dom;
using AngleSharp.Html.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.BlockChildren;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Block;

internal class TableConverter : ILegacyBlockConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "TABLE" };

    public IWemlBlockNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        IElement? head = element.QuerySelector("thead");
        IElement? tbody = element.QuerySelector("tbody");
        var result = new WemlTableElement();
        if (head is not null)
        {
            IEnumerable<IHtmlTableRowElement> rows = head.Children.OfType<IHtmlTableRowElement>();
            foreach (IHtmlTableRowElement? row in rows)
            {
                WemlTableRowNode parsedRow = ParseTableRow(parser, row, context);
                result.Header.Add(parsedRow);
            }
        }

        IEnumerable<IHtmlTableRowElement> bodyRows =
            tbody?.Children.OfType<IHtmlTableRowElement>() ?? element.Children.OfType<IHtmlTableRowElement>();
        foreach (IHtmlTableRowElement? row in bodyRows)
        {
            WemlTableRowNode parsedRow = ParseTableRow(parser, row, context);
            result.Body.Add(parsedRow);
        }
        return result;
    }


    private static WemlTableRowNode ParseTableRow(ILegacyElementParser parser,
        IHtmlTableRowElement rowElement,
        ILegacyParserContext context)
    {
        var result = new WemlTableRowNode(ArraySegment<WemlTableCellNode>.Empty);

        foreach (IHtmlTableCellElement child in rowElement.Cells)
        {
            var cell = new WemlTableCellNode(
                child.TagName == "TH",
                parser.ParseChildBlocks(child, context))
            {
                Align = child.GetAttribute("align") switch
                {
                    "left" => WemlHorizontalAlign.Left,
                    "right" => WemlHorizontalAlign.Right,
                    "center" => WemlHorizontalAlign.Center,
                    _ => null
                },
                Valign = child.GetAttribute("valign") switch
                {
                    "top" => WemlVerticalAlign.Top,
                    "bottom" => WemlVerticalAlign.Bottom,
                    "middle" => WemlVerticalAlign.Center,
                    _ => null
                }
            };

            if (child.HasAttribute("colspan") && int.TryParse(child.GetAttribute("colspan"), out int colspan) && colspan > 1)
            {
                cell.ColSpan = colspan;
            }

            if (child.HasAttribute("rowspan") && int.TryParse(child.GetAttribute("rowspan"), out int rowspan) && rowspan > 1)
            {
                cell.RowSpan = rowspan;
            }

            result.Cells.Add(cell);
        }

        return result;
    }
}