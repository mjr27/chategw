using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Container;

internal class ParagraphGroupContainerSerializer : IWemlJsonElementSerializer<WemlParagraphGroupContainer>
{
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlParagraphGroupContainer node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.ParagraphGroupContainer)
        .WithBooleanAttribute(Fields.Skip, node.Skip)
        .WithChildren(serializer, node.Children);


    public WemlParagraphGroupContainer? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.ParagraphContainer)
        {
            return null;
        }

        bool skip = htmlNode[Fields.Skip]?.GetValue<bool>() == true;
        return new WemlParagraphGroupContainer(
            htmlNode.GetChildren(deserializer).EnsureNodeTypes<WemlParagraphContainer>(htmlNode)
        ) { Skip = skip };
    }
}