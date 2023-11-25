using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class LineBreakConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "BR" };

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context) =>
        new WemlLineBreakNode();
}