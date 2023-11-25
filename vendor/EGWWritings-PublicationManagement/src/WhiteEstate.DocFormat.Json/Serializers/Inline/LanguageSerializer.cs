using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class LanguageSerializer : IWemlJsonElementSerializer<WemlLanguageElement>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlLanguageElement node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.Language)
        .WithAttribute("lang", node.Language)
        .WithEnumAttribute("dir", node.Direction)
        .WithChildren(serializer, node.Children);

    /// <inheritdoc />
    public WemlLanguageElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.Language)
        {
            return null;
        }

        string? lang = htmlNode["lang"]?.GetValue<string>();
        WemlTextDirection dir = htmlNode["dir"]?.GetValue<string>() switch
        {
            "rtl" => WemlTextDirection.RightToLeft,
            _ => WemlTextDirection.LeftToRight,
        };
        IWemlInlineNode[] children = htmlNode.GetChildren(deserializer).EnsureNodeTypes<IWemlInlineNode>(htmlNode);
        return new WemlLanguageElement(lang, dir, children);
    }
}