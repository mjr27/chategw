using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Block;

/// <inheritdoc />
public class FigureSerializer : IWemlJsonElementSerializer<WemlFigureElement>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlFigureElement node)
    {
        JsonObject obj = new JsonObject()
            .WithJsonNodeType(JsonNodeType.Image)
            .WithAttribute("src", node.Source)
            .WithAttribute("alt", node.AlternateText);
        if (node.Caption is not null)
        {
            obj[Fields.Header] = serializer.Serialize(node.Caption);
        }

        return obj;
    }

    /// <inheritdoc />
    public WemlFigureElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.Image)
        {
            return null;
        }

        string src = htmlNode["src"]?.GetValue<string>()
                     ?? throw new JsonDeserializationException($"Unable to parse image URI {htmlNode.ToJsonString()}", htmlNode);
        string? alt = htmlNode["alt"]?.GetValue<string>();
        WemlTextBlockElement? caption = null;
        if (htmlNode.TryGetPropertyValue(Fields.Header, out JsonNode? captionNode)
            && captionNode is JsonObject captionElement
            && deserializer.Deserialize(captionElement) is WemlTextBlockElement textBlock
           )
        {
            caption = textBlock;
        }

        return new WemlFigureElement(src, caption, alt);
    }
}