using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Converters;

internal class ObsoleteSpanFormattingConverter : ILegacyInlineConverter
{
    private static readonly string?[] SkippedClasses =
    {
        "nol-ink", "nolink", // missing links,
        "bibleversion", // DICTIONARY
        "sic", "x-tl", // remove obsolete
        "visibleInTOC", // TOPICAL INDEX
        "apparatus", // 14341
        "uncial", // NET Bible
        "originaltext", "correctedtext", "woj", // words of jesus, replace to <b></b>
    };

    public bool CanProcess(INode node) => node is IElement { NodeName: "SPAN" } el
                                          && SkippedClasses.Contains(el.ClassName);

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        context.Report(WarningLevel.Information, node, $"Skipping span with class {((IElement)node).ClassName}");
        return new WemlDummyInlineNode(parser.ParseChildInlines(node, context));
    }
}