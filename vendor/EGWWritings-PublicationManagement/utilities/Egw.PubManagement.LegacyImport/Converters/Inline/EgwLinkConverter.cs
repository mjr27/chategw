using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Links;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class EgwLinkConverter : ILegacyInlineConverter
{
    private readonly ILinkParser[] _linkParsers;

    public EgwLinkConverter(IEnumerable<ILinkParser> linkParsers)
    {
        _linkParsers = linkParsers.ToArray();
    }

    private static readonly string[] LinkClassesPrefixes = { "egw-", "bible-", "apl-", "ref-" };

    public bool CanProcess(INode node) =>
        node is IElement { NodeName: "SPAN" } e && e.ClassList.Any(cl => LinkClassesPrefixes.Any(cl.StartsWith));

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        IEnumerable<IWemlInlineNode> inlines = parser.ParseChildInlines(node, context);
        foreach (ILinkParser? linkParser in _linkParsers)
        {
            if (linkParser.TryResolveReference(element, context, out ParaId paraId, out bool isBible,
                    out string? title))
            {
                return new WemlLinkElement(
                    new WemlEgwLink(paraId, isBible, null),
                    inlines,
                    title
                );
            }
        }

        string? elementTitle = element.GetAttribute("data-title")
                               ?? element.GetAttribute("title");

        if (elementTitle is null)
        {
            return new WemlDummyInlineNode(inlines);
        }

        context.Report(WarningLevel.Warning, node, "Cannot resolve reference for link");

        return new WemlLinkElement(new WemlTemporaryLink(elementTitle), inlines, elementTitle);
    }
}