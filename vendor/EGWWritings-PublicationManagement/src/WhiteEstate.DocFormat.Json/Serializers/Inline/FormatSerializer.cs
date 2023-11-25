using System.Text.Json.Nodes;

using EnumsNET;

using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class FormatSerializer : IWemlJsonElementSerializer<WemlFormatElement>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlFormatElement node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.TextFormat)
        .WithSubtype(node.Formatting)
        .WithChildren(serializer, node.Children);

    /// <inheritdoc />
    public WemlFormatElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.TextFormat)
        {
            return null;
        }

        WemlTextFormatting format = EnumsNET.Enums.TryParse(
            htmlNode[Fields.Type]?.GetValue<string>(),
            true, out WemlTextFormatting formatting,
            EnumFormat.Description)
            ? formatting
            : throw new JsonDeserializationException("Invalid format type", htmlNode);
        IWemlInlineNode[] children = htmlNode.GetChildren(deserializer).EnsureNodeTypes<IWemlInlineNode>(htmlNode);
        return new WemlFormatElement(format, children);
    }
}