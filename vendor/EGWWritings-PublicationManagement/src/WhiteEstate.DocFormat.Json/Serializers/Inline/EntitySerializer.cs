using System.Text.Json.Nodes;

using EnumsNET;

using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class EntitySerializer : IWemlJsonElementSerializer<WemlEntityElement>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlEntityElement node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.Entity)
        .WithSubtype(node.EntityType)
        .WithAttribute(Fields.Value, node.Value)
        .WithChildren(serializer, node.Children);

    /// <inheritdoc />
    public WemlEntityElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.Entity)
        {
            return null;
        }

        WemlEntityType type = EnumsNET.Enums.TryParse(
            htmlNode[Fields.Type]?.GetValue<string>(),
            true, out WemlEntityType formatting,
            EnumFormat.Description)
            ? formatting
            : throw new JsonDeserializationException("Invalid entity type", htmlNode);
        string? value = htmlNode[Fields.Value]?.GetValue<string>();
        IWemlInlineNode[] children = htmlNode.GetChildren(deserializer).EnsureNodeTypes<IWemlInlineNode>(htmlNode);
        return new WemlEntityElement(type, children) { Value = value };
    }
}