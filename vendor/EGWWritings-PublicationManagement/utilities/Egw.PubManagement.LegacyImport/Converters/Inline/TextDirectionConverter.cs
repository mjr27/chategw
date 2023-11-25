using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class TextDirectionConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "SPAN" } element && element.HasAttribute("dir");

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        WemlTextDirection? direction = element.GetAttribute("dir") switch
        {
            "ltr" => WemlTextDirection.LeftToRight,
            "rtl" => WemlTextDirection.RightToLeft,
            _ => null
        };
        if (direction is not null)
        {
            return new WemlLanguageElement(null, direction.Value, parser.ParseChildInlines(node, context));
        }

        context.Report(WarningLevel.Error, node, "Invalid text direction specified");
        return new WemlDummyInlineNode(parser.ParseChildInlines(node, context));
    }
}