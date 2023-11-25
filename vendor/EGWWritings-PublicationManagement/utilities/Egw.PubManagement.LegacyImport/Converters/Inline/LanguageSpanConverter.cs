using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;
using Egw.PubManagement.LegacyImport.LinkRepository;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class LanguageSpanConverter : ILegacyInlineConverter
{
    private readonly ILinkRepository _linkRepository;

    public LanguageSpanConverter(ILinkRepository linkRepository)
    {
        _linkRepository = linkRepository;
    }

    public bool CanProcess(INode node) => node is IElement { NodeName: "SPAN" } el
                                          && (el.HasAttribute("lang") || el.HasAttribute("xml:lang"));

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        string? normalizedCode = _linkRepository.NormalizeLanguage(element.GetAttribute("lang")
                                                                   ?? element.GetAttribute("xml:lang")
                                                                   ?? "");
        IEnumerable<IWemlInlineNode> children = parser.ParseChildInlines(node, context);

        if (!string.IsNullOrWhiteSpace(normalizedCode))
        {
            return new WemlLanguageElement(normalizedCode, WemlTextDirection.LeftToRight, children);
        }

        context.Report(WarningLevel.Error, node, "Invalid language");
        return new WemlDummyInlineNode(children);
    }
}