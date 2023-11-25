using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class TextNodeConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IText;

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var textNode = (IText)node;
        return new WemlTextNode(textNode.NodeValue.Normalize());
    }
}