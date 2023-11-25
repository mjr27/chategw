using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Block;

/// <inheritdoc />
public class ThoughtBreakSerializer : IWemlJsonElementSerializer<WemlThoughtBreakElement>
{

    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlThoughtBreakElement node)
        => new JsonObject().WithJsonNodeType(JsonNodeType.ThoughtBreak);

    /// <inheritdoc />
    public WemlThoughtBreakElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
        => htmlNode.GetJsonNodeType() is JsonNodeType.ThoughtBreak ? new WemlThoughtBreakElement() : null;
}