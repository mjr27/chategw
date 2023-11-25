using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class NonEgwConverter : ILegacyInlineConverter
{
    private static readonly Dictionary<string, WemlNonEgwTextType> NonEgwClasses = new()
    {
        ["non-egw-appendix"] = WemlNonEgwTextType.Appendix,
        ["non-egw-comment"] = WemlNonEgwTextType.Comment,
        ["non-egw-foreword"] = WemlNonEgwTextType.Foreword,
        ["non-egw-intro"] = WemlNonEgwTextType.Intro,
        ["non-egw-note"] = WemlNonEgwTextType.Note,
        ["non-egw-preface"] = WemlNonEgwTextType.Preface,
        ["non-egw-text"] = WemlNonEgwTextType.Text,
        ["non-egw"] = WemlNonEgwTextType.Text, // small fix
    };


    public bool CanProcess(INode node) => node is IElement { NodeName: "SPAN" } el
                                          && NonEgwClasses.ContainsKey(el.ClassName ?? "");

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        if (NonEgwClasses.TryGetValue(element.ClassName!, out WemlNonEgwTextType nonEgwClass))
        {
            return new WemlNonEgwElement(nonEgwClass, parser.ParseChildInlines(node, context));
        }

        context.Report(WarningLevel.Error, node, "Invalid non-egw class name");
        return new WemlDummyInlineNode(parser.ParseChildInlines(node, context));
    }
}