using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters;

internal class ObsoleteTagsRemoveConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement { NodeName: "MAP" };

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        context.Report(WarningLevel.Information, node, "Removing obsolete tag");
        return new WemlDummyInlineNode(ArraySegment<IWemlInlineNode>.Empty);
    }
}