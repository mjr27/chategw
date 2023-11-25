using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Converters;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Internal;

internal static class InlineParserExtensions
{
    public static IEnumerable<IWemlInlineNode> ParseChildInlines(
        this ILegacyElementParser parser,
        INode node,
        ILegacyParserContext context)
    {
        if (node is not IElement)
        {
            context.Report(WarningLevel.Error, node, "Node is not element");
            yield break;
        }

        foreach (INode? child in node.ChildNodes)
        {
            IWemlInlineNode? result = parser.ParseInline(child, context);
            if (result is null)
            {
                context.Report(WarningLevel.Error, child, "Cannot find convertor for inline");
                yield break;
            }

            yield return result;
        }
    }

    // ReSharper disable once CognitiveComplexity
    public static IEnumerable<IWemlBlockNode> ParseChildBlocks(
        this ILegacyElementParser parser,
        INode node,
        ILegacyParserContext context)
    {
        if (node is not IElement)
        {
            context.Report(WarningLevel.Error, node, "Node is not element");
            yield break;
        }

        List<IWemlNode> children = new();
        foreach (INode? child in node.ChildNodes)
        {
            IWemlNode? result = parser.ParseInline(child, context)
                                ?? (IWemlNode?)parser.ParseBlock(child, context);

            if (result is null)
            {
                context.Report(WarningLevel.Error, child, "Cannot find inline convertor for child");
                children.Add(new WemlDummyInlineNode(
                    child.ChildNodes
                        .Select(n => parser.ParseInline(n, context))
                        .Where(n => n != null)
                        .Select(n => n!)
                ));
                yield break;
            }

            children.Add(result);
        }

        var inlines = new List<IWemlInlineNode>();
        foreach (IWemlNode subNode in children)
        {
            switch (subNode)
            {
                case IWemlInlineNode inlineSubNode:
                    inlines.Add(inlineSubNode);
                    break;
                case IWemlBlockNode blockSubNode:
                    if (inlines.Any())
                    {
                        yield return new WemlTextBlockElement(WemlParagraphType.Paragraph, inlines);
                        inlines = new List<IWemlInlineNode>();
                        yield return blockSubNode;
                    }

                    break;
            }
        }

        if (inlines.Any())
        {
            yield return new WemlTextBlockElement(WemlParagraphType.Paragraph, inlines);
        }
    }
}