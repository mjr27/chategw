using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class TopicalIndexHeadingConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "SPAN", ClassName: "mindex" or "lword" };

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var el = (IElement)node.Clone();
        IEnumerable<IWemlInlineNode> children = parser.ParseChildInlines(el, context);
        switch (el.ClassName)
        {
            case "mindex":
                return new WemlEntityElement(
                    WemlEntityType.Topic,
                    children
                );
            case "lword":
                return new WemlEntityElement(
                    WemlEntityType.TopicWord,
                    children
                );
            default:
                context.Report(WarningLevel.Error, node, "span.mindex may contain only single child span.lword");
                return new WemlDummyInlineNode(children);
        }
    }
}