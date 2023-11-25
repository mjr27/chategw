using System.Diagnostics.CodeAnalysis;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;

using Egw.PubManagement.LegacyImport.Internal;
using Egw.PubManagement.LegacyImport.LinkRepository;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Links;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class LinkConverter : ILegacyInlineConverter
{
    private readonly ILinkRepository _linkRepository;

    public LinkConverter(ILinkRepository linkRepository)
    {
        _linkRepository = linkRepository;
    }

    public bool CanProcess(INode node) =>
        node is IElement { NodeName: "A" } el && el.HasAttribute("href");

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        string? href = element.GetAttribute("href");
        if (href is null)
        {
            context.Report(WarningLevel.Error, node, "Unable to get href from a link");
            return new WemlDummyInlineNode(parser.ParseChildInlines(node, context));
        }

        string? title = element.GetAttribute("title");

        if (href.Contains('#'))
        {
            string anchor = href.StartsWith('#')
                ? context.Header.Code + href
                : href;

            string? para = _linkRepository.FindParagraph(anchor, "en-us");

            if (ParaId.TryParse(para, out ParaId paraId))
            {
                return new WemlLinkElement(
                    new WemlEgwLink(paraId, false, href.Split('#').Last()),
                    parser.ParseChildInlines(node, context),
                    title
                );
            }

            if (href.StartsWith('#'))
            {
                string anchorName = href[1..];
                if (TryFindAnchorInDocument(anchorName, context.Document, out int anchorParaId))
                {
                    return new WemlLinkElement(
                        new WemlEgwLink(new ParaId(context.Header.Id, anchorParaId), false, anchorName),
                        parser.ParseChildInlines(node, context),
                        title
                    );
                }
            }
        }
        else if (TryConvertExternalLink(parser, context, node, href, title, out IWemlInlineNode? convert))
        {
            return convert;
        }

        context.Report(WarningLevel.Warning, node, "Unable to parse external link");
        return new WemlDummyInlineNode(parser.ParseChildInlines(node, context));
    }

    private static bool TryConvertExternalLink(
        ILegacyElementParser parser,
        ILegacyParserContext context,
        INode node,
        string href,
        string? title,
        [NotNullWhen(true)] out IWemlInlineNode? outputNode)
    {
        outputNode = null;
        if (!Uri.TryCreate(href, UriKind.Absolute, out Uri? externalUri))
        {
            return false;
        }

        switch (externalUri.Scheme)
        {
            case "mailto":
                {
                    outputNode = new WemlLinkElement(
                        new WemlEmailLink(externalUri.UserInfo + "@" + externalUri.Host),
                        parser.ParseChildInlines(node, context),
                        title
                    );
                    return true;
                }
            case "http" or "https":
                {
                    outputNode = new WemlLinkElement(
                        new WemlExternalLink(externalUri),
                        parser.ParseChildInlines(node, context),
                        title
                    );
                    return true;
                }
        }

        return false;
    }

    private static bool TryFindAnchorInDocument(
        string anchorName,
        IParentNode document,
        out int elementId)
    {
        elementId = default;
        IEnumerable<IHtmlAnchorElement> anchors = document.QuerySelectorAll<IHtmlAnchorElement>("a[name]");
        foreach (IHtmlAnchorElement anchor in anchors)
        {
            string? currentAnchorName = anchor.GetAttribute("name");
            if (currentAnchorName != anchorName)
            {
                continue;
            }

            IElement currentElement = anchor;
            if (TryFindElementId(currentElement, out elementId))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryFindElementId(IElement? currentElement, out int elementId)
    {
        elementId = default;
        while (currentElement != null)
        {
            string? paragraphId = currentElement.GetAttribute("id");
            if (paragraphId != null && paragraphId.Contains('.'))
            {
                string elementIdStr = paragraphId.Split('.').Last();
                if (int.TryParse(elementIdStr, out elementId))
                {
                    return true;
                }
            }

            currentElement = currentElement.ParentElement;
        }

        return false;
    }
}