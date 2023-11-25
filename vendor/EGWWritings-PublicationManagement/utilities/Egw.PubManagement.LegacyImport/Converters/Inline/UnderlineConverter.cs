using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class UnderlineConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node
        is IElement { NodeName: "U", ClassName: null or "" }
        or IElement { NodeName: "SPAN", ClassName: "underline" };

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context) =>
        new WemlFormatElement(WemlTextFormatting.Underline, parser.ParseChildInlines(node, context));
}