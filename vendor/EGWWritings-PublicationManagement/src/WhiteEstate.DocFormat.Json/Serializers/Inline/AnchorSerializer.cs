using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class AnchorSerializer : IWemlJsonElementSerializer<WemlAnchorNode>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlAnchorNode node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.Anchor)
        .WithAttribute(Fields.Id, node.UniqueId);

    /// <inheritdoc />
    public WemlAnchorNode? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
        => htmlNode.GetJsonNodeType() is JsonNodeType.Anchor
            ? new WemlAnchorNode(htmlNode[Fields.Id]?.GetValue<string>()
                                 ?? throw new JsonDeserializationException("Unique anchor id is required", htmlNode))
            : null;
}