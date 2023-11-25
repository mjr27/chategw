using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters;

internal class ObsoleteTagsConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "ABBR" };

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        context.Report(WarningLevel.Information, node, "Skipping tag");
        return new WemlDummyInlineNode(parser.ParseChildInlines(node, context));
    }
}