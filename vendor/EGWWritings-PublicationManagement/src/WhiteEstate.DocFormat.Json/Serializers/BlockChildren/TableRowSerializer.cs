using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.BlockChildren;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.BlockChildren;

/// <inheritdoc />
public class TableRowSerializer : IWemlJsonElementSerializer<WemlTableRowNode>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlTableRowNode node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.TableRow)
        .WithChildren(serializer, node.Cells);

    /// <inheritdoc />
    public WemlTableRowNode? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode) =>
        htmlNode.GetJsonNodeType() is JsonNodeType.TableRow
            ? new WemlTableRowNode(
                htmlNode.GetChildren(deserializer).EnsureNodeTypes<WemlTableCellNode>(htmlNode)
            )
            : null;
}