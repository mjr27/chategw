using System.Text.Json.Nodes;

using EnumsNET;

using WhiteEstate.DocFormat.BlockChildren;
using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Block;

/// <inheritdoc />
public class ListSerializer : IWemlJsonElementSerializer<WemlListElement>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlListElement node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.List)
        .WithSubtype(node.ListType)
        .WithChildren(serializer, node.Children);

    /// <inheritdoc />
    public WemlListElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.List)
        {
            return null;
        }

        WemlListType type = EnumsNET.Enums.TryParse(
            htmlNode[Fields.Type]?.GetValue<string>(),
            true, out WemlListType formatting,
            EnumFormat.Description)
            ? formatting
            : throw new JsonDeserializationException("Invalid list type", htmlNode);
        WemlListItemNode[] children = htmlNode.GetChildren(deserializer).EnsureNodeTypes<WemlListItemNode>(htmlNode);
        return new WemlListElement(type, children);
    }
}