using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters;

internal class EmptySpanFormattingConverter : ILegacyInlineConverter
{
    private static readonly string?[] AllowedAttributes = { "style" };


    public bool CanProcess(INode node) => node is IElement { NodeName: "SPAN" } el
                                          && el.Attributes.All(attr => AllowedAttributes.Contains(attr.Name));

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        return new WemlDummyInlineNode(parser.ParseChildInlines(node, context));
    }
}