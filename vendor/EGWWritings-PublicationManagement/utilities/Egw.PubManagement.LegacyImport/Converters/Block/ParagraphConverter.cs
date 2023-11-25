using AngleSharp.Dom;
using AngleSharp.Html.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Block;

internal class ParagraphConverter : ILegacyBlockConverter
{
    private enum BlockType
    {
        Paragraph,
        Blockquote,
        Poem,
        ThoughtBreak,
        Picture
    }

    public bool CanProcess(INode node) => node is IElement
                                          {
                                              NodeName: "P" or "DIV" or "H1" or "H2" or "H3" or "H4" or "H5" or "H6"
                                          } el
                                          && !el.ClassList.Contains("page");

    public IWemlBlockNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var el = (IElement)node;
        BlockType blockType = GetParagraphAttributes(el);

        switch (blockType)
        {
            case BlockType.Paragraph:
                return new WemlTextBlockElement(WemlParagraphType.Paragraph, parser.ParseChildInlines(node, context));
            case BlockType.Blockquote:
                return new WemlTextBlockElement(WemlParagraphType.Blockquote, parser.ParseChildInlines(node, context));
            case BlockType.Poem:
                return new WemlTextBlockElement(WemlParagraphType.Poem, parser.ParseChildInlines(node, context));
            case BlockType.Picture:
                var newNode = (IElement)el.Clone();
                if (newNode.QuerySelector("a[href]") is not IHtmlAnchorElement link)
                {
                    context.Report(WarningLevel.Error, newNode, "Picture does not contain link");
                    return new WemlTextBlockElement(WemlParagraphType.Paragraph, ArraySegment<IWemlInlineNode>.Empty);
                }

                link.Remove();
                IWemlInlineNode[] inlines = parser.ParseChildInlines(newNode, context).ToArray();
                return new WemlFigureElement(
                    NormalizeImageUri(link.Href),
                    inlines.Length > 0
                        ? new WemlTextBlockElement(WemlParagraphType.Paragraph, inlines)
                        : null,
                    link.TextContent.Trim()
                );
            case BlockType.ThoughtBreak:
                return new WemlThoughtBreakElement();
            default:
                context.Report(WarningLevel.Error, node, $"Invalid block type: {blockType}");
                return new WemlTextBlockElement(WemlParagraphType.Paragraph, ArraySegment<IWemlInlineNode>.Empty);
        }
    }

    private static string NormalizeImageUri(string href)
    {
        var uri = new Uri(href);
        if (uri.Scheme == "about")
        {
            uri = new Uri(new Uri("https://media4.egwwritings.org"), uri.AbsolutePath);
        }

        return uri.AbsoluteUri;
    }

    // ReSharper disable once CognitiveComplexity
    private BlockType GetParagraphAttributes(IElement element)
    {
        BlockType blockType = BlockType.Paragraph;
        foreach (string? className in element.ClassList)
        {
            switch (className)
            {
                case "blockquote-noindent":
                case "blockquote-insideaddr":
                case "blockquote-indented":
                case "blockquote-reverse":
                case "blockquote-center":
                case "introquote":
                    blockType = BlockType.Blockquote;
                    break;
                case "poem-indented":
                case "poem-noindent":
                    blockType = BlockType.Poem;
                    break;
                case "thoughtbreak":
                    blockType = BlockType.ThoughtBreak;
                    break;
                case "picture":
                case "wrap" or "nowrap" or "wrap-start" or "wrap-end": // EGWEnc
                    blockType = BlockType.Picture;
                    break;
            }
        }

        return blockType;
    }
}