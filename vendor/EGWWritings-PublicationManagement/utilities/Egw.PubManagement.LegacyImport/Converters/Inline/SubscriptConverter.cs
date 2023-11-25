using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class SubscriptConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "SUB" } el && string.IsNullOrEmpty(el.ClassName);

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        return new WemlFormatElement(WemlTextFormatting.Subscript, parser.ParseChildInlines(node, context));
    }
}