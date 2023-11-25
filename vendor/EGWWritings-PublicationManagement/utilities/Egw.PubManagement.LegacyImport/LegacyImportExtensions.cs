using AngleSharp.Dom;
using AngleSharp.Html.Dom;

using Egw.PubManagement.LegacyImport;
using Egw.PubManagement.LegacyImport.Converters;
using Egw.PubManagement.LegacyImport.Converters.Block;
using Egw.PubManagement.LegacyImport.Converters.Container;
using Egw.PubManagement.LegacyImport.Converters.Inline;
using Egw.PubManagement.LegacyImport.Internal;
using Egw.PubManagement.LegacyImport.LinkParsing;

using WhiteEstate.DocFormat.Serialization;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Dependency injection methods
/// </summary>
public static class LegacyImportExtensions
{
    private class DummyTextNodeSerializer : IWemlElementSerializer<WemlDummyInlineNode>
    {
        public INode Serialize(IHtmlDocument document, IWemlSerializer serializer, WemlDummyInlineNode node)
        {
            IDocumentFragment fragment = document.CreateDocumentFragment();
            fragment.AppendNodes(node.Children.Select(serializer.Serialize).ToArray());
            return fragment;
        }

        public WemlDummyInlineNode? Deserialize(IWemlDeserializer _, INode htmlNode) => null;
    }

    /// <summary>
    /// Enriches serializer with legacy fragments
    /// </summary>
    /// <param name="serializer"></param>
    public static void EnrichWithLegacy(this WemlSerializer serializer)
    {
        serializer.Register(new DummyTextNodeSerializer());
    }

    /// <summary>
    /// Adds legacy import services to the service collection
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddLegacyImports(
        this IServiceCollection services)
    {
        services.AddSingleton<ILinkParser, ParaIdLinkParserImpl>();
        services.AddSingleton<ILinkParser, BookLinkParserImpl>();
        services.AddSingleton<ILinkParser, BibleLinkParserImpl>();
        services.AddSingleton<ILinkParser, AnchorLinkParserImpl>();

        services.AddSingleton<ILegacyInlineConverter, TextNodeConverter>();
        services.AddSingleton<ILegacyInlineConverter, LineBreakConverter>();
        services.AddSingleton<ILegacyInlineConverter, PageBreakConverter>();
        services.AddSingleton<ILegacyInlineConverter, BoldConverter>();
        services.AddSingleton<ILegacyInlineConverter, ItalicConverter>();
        services.AddSingleton<ILegacyInlineConverter, EgwLinkConverter>();
        services.AddSingleton<ILegacyInlineConverter, LinkConverter>();
        services.AddSingleton<ILegacyInlineConverter, NonEgwConverter>();
        services.AddSingleton<ILegacyInlineConverter, NoteConverter>();
        services.AddSingleton<ILegacyInlineConverter, SuperscriptConverter>();
        services.AddSingleton<ILegacyInlineConverter, SubscriptConverter>();
        services.AddSingleton<ILegacyInlineConverter, DictionaryWordConverter>();
        services.AddSingleton<ILegacyInlineConverter, TopicalIndexHeadingConverter>();
        services.AddSingleton<ILegacyInlineConverter, UnderlineConverter>();

        services.AddSingleton<ILegacyInlineConverter, AnchorConverter>();
        services.AddSingleton<ILegacyInlineConverter, FormattingConverter>();
        services.AddSingleton<ILegacyInlineConverter, EntityTypeConverter>();
        services.AddSingleton<ILegacyInlineConverter, TextDirectionConverter>();
        services.AddSingleton<ILegacyInlineConverter, RubyTextConverter>();

        services.AddSingleton<ILegacyInlineConverter, EmptySpanFormattingConverter>();
        services.AddSingleton<ILegacyInlineConverter, LanguageSpanConverter>();
        services.AddSingleton<ILegacyInlineConverter, ObsoleteSpanFormattingConverter>();
        services.AddSingleton<ILegacyInlineConverter, ObsoleteTagsConverter>();
        services.AddSingleton<ILegacyInlineConverter, ObsoleteTagsRemoveConverter>();

        services.AddSingleton<ILegacyInlineConverter, NewPageConverter>();
        services.AddSingleton<ILegacyBlockConverter, ParagraphConverter>();
        services.AddSingleton<ILegacyBlockConverter, TableConverter>();
        services.AddSingleton<ILegacyBlockConverter, ListConverter>();

        services.AddSingleton<ILegacyContainerConverter, HeadingContainerConverter>();
        services.AddSingleton<ILegacyContainerConverter, PageBreakContainerConverter>();
        services.AddSingleton<ILegacyContainerConverter, ParagraphContainerConverter>();
        services.AddSingleton<ILegacyContainerConverter, ListContainerConverter>();
        services.AddSingleton<ILegacyContainerConverter, TableContainerConverter>();
        services.AddSingleton<ILegacyElementParser, LegacyElementParserImpl>();
        return services;
    }
}