using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class RubyTextConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "SPAN", ClassName: "ruby" };

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node.Clone();
        IElement? rubyText = element.QuerySelector(".rt");
        if (rubyText is null)
        {
            context.Report(WarningLevel.Error, node, "Ruby text contains no span.rt child");
            return new WemlDummyInlineNode();
        }

        rubyText.Remove();
        return new WemlTextNode(element.TextContent);
    }
}