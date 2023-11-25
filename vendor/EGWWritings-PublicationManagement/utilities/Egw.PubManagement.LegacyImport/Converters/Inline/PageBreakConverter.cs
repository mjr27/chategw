using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class PageBreakConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "SPAN" } e
                                          && (e.ClassList.Contains("page-break")
                                              || e.ClassList.Contains("pagebreak")
                                              || e.ClassList.Contains("pagenumber")
                                              || e.ClassList.Contains("page"));

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        string? pageNumber = null;
        if (element.ClassList.Contains("pagenumber") || element.ClassList.Contains("page"))
        {
            pageNumber = element.Text().Trim();
        }

        if (!string.IsNullOrWhiteSpace(pageNumber))
        {
            return new WemlPageBreakElement(pageNumber);
        }

        context.Report(WarningLevel.Error, node, "Page break contains no page marker");
        return new WemlDummyInlineNode();

    }
}