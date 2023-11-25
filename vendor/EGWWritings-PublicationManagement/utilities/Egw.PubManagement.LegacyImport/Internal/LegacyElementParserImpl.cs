using AngleSharp.Dom;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.LegacyImport.Internal;

internal class LegacyElementParserImpl : ILegacyElementParser
{
    private readonly ILegacyInlineConverter[] _inlineConverters;
    private readonly ILegacyBlockConverter[] _blockConverters;
    private readonly ILegacyContainerConverter[] _containerConverters;

    public LegacyElementParserImpl(
        IEnumerable<ILegacyInlineConverter> inlineConverters,
        IEnumerable<ILegacyBlockConverter> blockConverters,
        IEnumerable<ILegacyContainerConverter> containerConverters
    )
    {
        _inlineConverters = inlineConverters.ToArray();
        _blockConverters = blockConverters.ToArray();
        _containerConverters = containerConverters.ToArray();
    }


    public IWemlInlineNode? ParseInline(INode node, ILegacyParserContext context)
    {
        foreach (ILegacyInlineConverter? converter in _inlineConverters)
        {
            if (!converter.CanProcess(node))
            {
                continue;
            }

            return converter.Convert(this, node, context);
        }

        return null;
    }

    public IWemlBlockNode? ParseBlock(INode node, ILegacyParserContext context)
    {
        foreach (ILegacyBlockConverter? converter in _blockConverters)
        {
            if (!converter.CanProcess(node))
            {
                continue;
            }

            return converter.Convert(this, node, context);
        }

        return null;
    }

    public IWemlContainerElement? ParseContainer(INode node, ILegacyParserContext context)
    {
        foreach (ILegacyContainerConverter? converter in _containerConverters)
        {
            if (!converter.CanProcess(node))
            {
                continue;
            }

            return converter.Convert(this, node, context);
        }

        return null;
    }
}