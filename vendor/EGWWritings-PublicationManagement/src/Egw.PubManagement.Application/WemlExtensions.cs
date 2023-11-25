using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat.Serialization;

using WhiteEstate.DocFormat.Containers;

// ReSharper disable once CheckNamespace
namespace WhiteEstate.DocFormat;

internal static class WemlExtensions
{
    private static IWemlDeserializer DefaultDeserializer { get; set; } = new WemlDeserializer();
    private static IHtmlParser DefaultHtmlParser { get; } = new HtmlParser();
    private static IHtmlElement Body { get; } = DefaultHtmlParser.ParseDocument("").Body!;

    public static IWemlContainerElement DeserializedContent(this Paragraph paragraph,
        IWemlDeserializer? deserializer = null)
    {
        deserializer ??= DefaultDeserializer;
        INodeList fragment = DefaultHtmlParser.ParseFragment(paragraph.Content, Body);
        return fragment
            .OfType<IElement>()
            .Select(deserializer.Deserialize)
            .OfType<IWemlContainerElement>()
            .Single();
    }

    public static IElement DeserializeToHtml(this Paragraph paragraph)
    {
        INodeList fragment = DefaultHtmlParser.ParseFragment(paragraph.Content, Body);
        return fragment
            .OfType<IElement>()
            .First();
    }

    public static async Task<bool> ValidateAsync(String content, CancellationToken cancellationToken)
    {
        return await Task.Run(() => Validate(content));
    }

    public static bool Validate(String content)
    {
        INodeList fragment = DefaultHtmlParser.ParseFragment(content, Body);
        return fragment.Length == 1 && IsWemlValid(DefaultDeserializer, fragment.Single());
    }

    private static bool IsWemlValid(IWemlDeserializer deserializer, INode node)
    {
        IWemlNode? weml;
        try
        {
            weml = deserializer.Deserialize(node);
        }
        catch (DeserializationException)
        {
            return false;
        }

        return weml is IWemlContainerElement;
    }

    public static int? GetHeadingLevel(String content)
    {
        INodeList fragment = DefaultHtmlParser.ParseFragment(content, Body);
        IWemlNode? weml = DefaultDeserializer.Deserialize(fragment.Single());
        if (weml is not null)
        {
            if (weml is WemlPageBreakElement)
            {
                return null;
            }
            else if (weml is WemlParagraphContainer || weml is WemlParagraphGroupContainer)
            {
                return 0;
            }
            else if (weml is WemlHeadingContainer)
            {
                return  (weml as WemlHeadingContainer)!.Level;
            }
        }

        //so if some another type??
        return int.MaxValue;
    }

    public static bool IsReferenced(String content)
    {
        INodeList fragment = DefaultHtmlParser.ParseFragment(content, Body);
        IWemlNode? weml = DefaultDeserializer.Deserialize(fragment.Single());
        if (weml is not null)
        {
            if (weml is WemlPageBreakElement)
            {
                return false;
            }
            else if (weml is WemlParagraphContainer)
            {
                return !(weml as WemlParagraphContainer)!.Skip;
            }
            else if (weml is WemlParagraphGroupContainer)
            {
                return !(weml as WemlParagraphGroupContainer)!.Skip;
            }
            else if (weml is WemlHeadingContainer)
            {
                return !(weml as WemlHeadingContainer)!.Skip;
            }
        }

        return false;
    }
}