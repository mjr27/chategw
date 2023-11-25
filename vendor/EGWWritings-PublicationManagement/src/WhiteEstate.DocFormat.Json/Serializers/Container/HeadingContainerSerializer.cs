using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Container;

internal class HeadingContainerSerializer : IWemlJsonElementSerializer<WemlHeadingContainer>
{
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlHeadingContainer node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.HeadingContainer)
        .WithIntAttribute(Fields.Level, node.Level)
        .WithBooleanAttribute(Fields.Skip, node.Skip)
        .WithChild(serializer.Serialize(node.Child));

    public WemlHeadingContainer? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.HeadingContainer)
        {
            return null;
        }

        bool skip = htmlNode[Fields.Skip]?.GetValue<bool>() == true;
        int level = htmlNode[Fields.Level]?.GetValue<int>() ?? 0;
        return new WemlHeadingContainer(level, htmlNode.GetChildren(deserializer).EnsureNodeTypes<WemlTextBlockElement>(htmlNode).Single())
        {
            Skip = skip
        };
    }
}