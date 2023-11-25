using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class SuperscriptConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "SUP" } el && string.IsNullOrEmpty(el.ClassName);

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        return new WemlFormatElement(WemlTextFormatting.Superscript, parser.ParseChildInlines(node, context));
    }
}