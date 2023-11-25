using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class ItalicConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "I" or "EM" } el
                                          && string.IsNullOrWhiteSpace(el.ClassName);

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context) =>
        new WemlFormatElement(WemlTextFormatting.Italic, parser.ParseChildInlines(node, context));
}