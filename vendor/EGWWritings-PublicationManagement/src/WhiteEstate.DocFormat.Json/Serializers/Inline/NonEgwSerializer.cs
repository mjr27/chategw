using System.Text.Json.Nodes;

using EnumsNET;

using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class NonEgwSerializer : IWemlJsonElementSerializer<WemlNonEgwElement>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlNonEgwElement node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.NonEgw)
        .WithSubtype(node.TextType)
        .WithChildren(serializer, node.Children);

    /// <inheritdoc />
    public WemlNonEgwElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.NonEgw)
        {
            return null;
        }

        WemlNonEgwTextType format = EnumsNET.Enums.TryParse(
            htmlNode[Fields.Type]?.GetValue<string>(),
            true, out WemlNonEgwTextType formatting,
            EnumFormat.Description)
            ? formatting
            : throw new JsonDeserializationException("Invalid non-egw type", htmlNode);
        IWemlInlineNode[] children = htmlNode.GetChildren(deserializer).EnsureNodeTypes<IWemlInlineNode>(htmlNode);
        return new WemlNonEgwElement(format, children);
    }
}