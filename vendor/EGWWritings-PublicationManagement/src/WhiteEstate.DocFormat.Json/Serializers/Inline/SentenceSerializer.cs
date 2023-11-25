using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class SentenceSerializer : IWemlJsonElementSerializer<WemlSentenceElement>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlSentenceElement node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.Sentence)
        .WithChildren(serializer, node.Children);

    /// <inheritdoc />
    public WemlSentenceElement? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode) =>
        htmlNode.GetJsonNodeType() is JsonNodeType.Sentence
            ? new WemlSentenceElement(
                htmlNode.GetChildren(deserializer).EnsureNodeTypes<IWemlInlineNode>(htmlNode)
            )
            : null;

}