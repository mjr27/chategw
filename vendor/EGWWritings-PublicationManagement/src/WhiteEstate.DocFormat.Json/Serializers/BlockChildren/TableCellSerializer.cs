using System.Text.Json.Nodes;

using EnumsNET;

using WhiteEstate.DocFormat.BlockChildren;
using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.BlockChildren;

/// <inheritdoc />
public class TableCellSerializer : IWemlJsonElementSerializer<WemlTableCellNode>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlTableCellNode node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.TableCell)
        .WithBooleanAttribute(Fields.Header, node.Header)
        .WithEnumAttribute(Fields.HorizontalAlign, node.Align)
        .WithEnumAttribute(Fields.VerticalAlign, node.Valign)
        .WithIntAttribute("rowspan", node.RowSpan > 1 ? node.RowSpan : null)
        .WithIntAttribute("colspan", node.ColSpan > 1 ? node.ColSpan : null)
        .WithChildren(serializer, node.Children);

    /// <inheritdoc />
    public WemlTableCellNode? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.TableCell)
        {
            return null;
        }

        WemlHorizontalAlign? align = EnumsNET.Enums.TryParse(
            htmlNode[Fields.HorizontalAlign]?.GetValue<string>(),
            true, out WemlHorizontalAlign val,
            EnumFormat.Description)
            ? val
            : null;
        WemlVerticalAlign? valign = EnumsNET.Enums.TryParse(
            htmlNode[Fields.VerticalAlign]?.GetValue<string>(),
            true, out WemlVerticalAlign val2,
            EnumFormat.Description)
            ? val2
            : null;
        return new WemlTableCellNode(
            htmlNode.ContainsKey(Fields.Header) && htmlNode[Fields.Header]?.GetValue<bool>() == true,
            htmlNode.GetChildren(deserializer).EnsureNodeTypes<IWemlBlockNode>(htmlNode)
        )
        {
            RowSpan = htmlNode["rowspan"]?.GetValue<int>() ?? 1,
            ColSpan = htmlNode["colspan"]?.GetValue<int>() ?? 1,
            Align = align,
            Valign = valign,
        };
    }
}