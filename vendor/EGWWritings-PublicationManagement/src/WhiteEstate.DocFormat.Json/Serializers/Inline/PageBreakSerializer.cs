using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class PageBreakSerializer : IWemlJsonElementSerializer<WemlPageBreakElement>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlPageBreakElement node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.PageBreak)
        .WithAttribute(Fields.PageNumber, node.Page);

    /// <inheritdoc />
    public WemlPageBreakElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
        => htmlNode.GetJsonNodeType() is JsonNodeType.PageBreak
            ? new WemlPageBreakElement(
                htmlNode[Fields.PageNumber]?.GetValue<string>()!
            )
            : null;
}