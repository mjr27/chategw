using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.BlockChildren;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.BlockChildren;

/// <inheritdoc />
public class ListItemSerializer : IWemlJsonElementSerializer<WemlListItemNode>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlListItemNode node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.ListItem)
        .WithChildren(serializer, node.Children);

    /// <inheritdoc />
    public WemlListItemNode? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
        => htmlNode.GetJsonNodeType() is JsonNodeType.ListItem
            ? new WemlListItemNode(
                htmlNode.GetChildren(deserializer).EnsureNodeTypes<IWemlBlockNode>(htmlNode)
            )
            : null;

}