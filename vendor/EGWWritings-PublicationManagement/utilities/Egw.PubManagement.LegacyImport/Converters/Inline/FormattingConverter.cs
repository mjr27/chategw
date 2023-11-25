using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class FormattingConverter : ILegacyInlineConverter
{
    private static readonly Dictionary<string, WemlTextFormatting> Formatting = new()
    {
        ["caps"] = WemlTextFormatting.AllCaps,
        ["small-caps"] = WemlTextFormatting.SmallCaps,
        ["ItalicText"] = WemlTextFormatting.Italic,
        ["SuperscriptText"] = WemlTextFormatting.Superscript,
        ["SmallCapsText"] = WemlTextFormatting.SmallCaps
    };

    public bool CanProcess(INode node) => node is IElement
    {
        NodeName: "SPAN",
    } el && Formatting.ContainsKey(el.ClassName ?? "");

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        return new WemlFormatElement(
            Formatting[element.ClassName ?? ""],
            parser.ParseChildInlines(node, context)
        );
    }
}