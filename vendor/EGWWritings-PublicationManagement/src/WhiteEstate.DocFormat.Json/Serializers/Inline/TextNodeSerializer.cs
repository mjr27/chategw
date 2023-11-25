using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class TextNodeSerializer : IWemlJsonElementSerializer<WemlTextNode>
{

    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlTextNode node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.TextNode)
        .WithAttribute(Fields.Content, node.Text);

    /// <inheritdoc />
    public WemlTextNode? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode) =>
        htmlNode.GetJsonNodeType() is JsonNodeType.TextNode
            ? new WemlTextNode(htmlNode[Fields.Content]?.GetValue<string>()
                               ?? throw new JsonDeserializationException("Missing content", htmlNode))
            : null;
}