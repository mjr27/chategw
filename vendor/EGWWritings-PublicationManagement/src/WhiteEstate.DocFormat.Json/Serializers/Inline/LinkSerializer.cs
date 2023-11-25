using System.Net;
using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Json.Internal;
using WhiteEstate.DocFormat.Links;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class LinkSerializer : IWemlJsonElementSerializer<WemlLinkElement>
{


    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlLinkElement node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.Link)
        .WithAttribute("href", node.Link.Reference)
        .WithAttribute("title", node.Title)
        .WithChildren(serializer, node.Children);

    /// <inheritdoc />
    public WemlLinkElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.Link)
        {
            return null;
        }

        IWemlLink? link = ParseHref(htmlNode);
        if (link is null)
        {
            throw new JsonDeserializationException("Unable to deserialize link", htmlNode);
        }

        return new WemlLinkElement(
            link,
            htmlNode.GetChildren(deserializer).EnsureNodeTypes<IWemlInlineNode>(htmlNode),
            htmlNode["title"]?.GetValue<string>()
        );
    }

    private static IWemlLink? ParseHref(JsonObject obj)
    {
        if (!Uri.TryCreate(obj["href"]?.GetValue<string>(), UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            return null;
        }

        switch (uri)
        {
            case { Scheme: "http" or "https", IsAbsoluteUri: true }:
                return new WemlExternalLink(uri);
            case { Scheme: "mailto" }:
                return new WemlEmailLink(uri.AbsoluteUri);
            case { Scheme: "egw", IsAbsoluteUri: true, Host: "bible" or "book" }:
                {
                    string bookType = uri.Host;
                    string elementIdPart = uri.AbsolutePath.Trim('/');
                    string anchor = uri.Fragment.TrimStart('#');

                    if (!ParaId.TryParse(elementIdPart, out ParaId paraId))
                    {
                        return null;
                    }

                    return new WemlEgwLink(
                        paraId,
                        bookType switch
                        {
                            "bible" => true,
                            "book" => false,
                            _ => throw new JsonDeserializationException("Unknown link type", obj)
                        },
                        anchor
                    );
                }
            case { Scheme: "egw", IsAbsoluteUri: true, Host: "missing" }:
                {
                    return new WemlTemporaryLink(WebUtility.UrlDecode(uri.AbsolutePath.TrimStart('/')));
                }
            default:
                throw new JsonDeserializationException("Cannot parse link", obj);
        }
    }
}