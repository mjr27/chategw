using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class BoldConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "B" or "STRONG" } el
                                          && string.IsNullOrWhiteSpace(el.ClassName);

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context) =>
        new WemlFormatElement(WemlTextFormatting.Bold, parser.ParseChildInlines(node, context));
}