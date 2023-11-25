using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class LineBreakSerializer : IWemlJsonElementSerializer<WemlLineBreakNode>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlLineBreakNode node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.LineBreak);

    /// <inheritdoc />
    public WemlLineBreakNode? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
        => htmlNode.GetJsonNodeType() is JsonNodeType.LineBreak
            ? new WemlLineBreakNode()
            : null;
}