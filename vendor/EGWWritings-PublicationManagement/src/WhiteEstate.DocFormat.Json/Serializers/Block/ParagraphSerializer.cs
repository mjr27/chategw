using System.Text.Json.Nodes;

using EnumsNET;

using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Block;

/// <inheritdoc />
public class ParagraphSerializer : IWemlJsonElementSerializer<WemlTextBlockElement>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlTextBlockElement node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.Paragraph)
        .WithSubtype(node.Type)
        .WithChildren(serializer, node.Children);

    /// <inheritdoc />
    public WemlTextBlockElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.Paragraph)
        {
            return null;
        }

        WemlParagraphType type = EnumsNET.Enums.TryParse(
            htmlNode[Fields.Type]?.GetValue<string>(),
            true, out WemlParagraphType formatting,
            EnumFormat.Description)
            ? formatting
            : throw new JsonDeserializationException("Invalid paragraph type", htmlNode);
        IWemlInlineNode[] children = htmlNode.GetChildren(deserializer).EnsureNodeTypes<IWemlInlineNode>(htmlNode);
        return new WemlTextBlockElement(type, children);
    }
}