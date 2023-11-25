using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class DictionaryWordConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) =>
        node is IElement { NodeName: "SPAN", ClassName: "h3" or "h4" };

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var el = (IElement)node.Clone();

        IEnumerable<IWemlInlineNode> inlines = parser.ParseChildInlines(node, context);
        return new WemlEntityElement(el.ClassName switch
        {
            "h3" => WemlEntityType.Topic,
            "h4" => WemlEntityType.TopicWord,
            _ => throw new ArgumentOutOfRangeException()
        }, inlines);
    }
}