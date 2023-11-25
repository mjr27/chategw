using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.BlockChildren;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Block;

/// <inheritdoc />
public class TableSerializer : IWemlJsonElementSerializer<WemlTableElement>
{

    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlTableElement node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.Table)
        .WithChildren(serializer, node.Header, Fields.Header)
        .WithChildren(serializer, node.Body);

    /// <inheritdoc />
    public WemlTableElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.ThoughtBreak)
        {
            return null;
        }

        var table = new WemlTableElement();
        foreach (WemlTableRowNode row in htmlNode.GetChildren(deserializer, Fields.Header).EnsureNodeTypes<WemlTableRowNode>(htmlNode))
        {
            table.Header.Add(row);
        }

        foreach (WemlTableRowNode row in htmlNode.GetChildren(deserializer).EnsureNodeTypes<WemlTableRowNode>(htmlNode))
        {
            table.Body.Add(row);
        }

        return table;
    }
}