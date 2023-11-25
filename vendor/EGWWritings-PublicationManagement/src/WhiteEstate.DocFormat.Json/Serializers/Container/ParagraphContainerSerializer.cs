using System.Text.Json.Nodes;

using EnumsNET;

using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Container;

internal class ParagraphContainerSerializer : IWemlJsonElementSerializer<WemlParagraphContainer>
{
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlParagraphContainer node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.ParagraphContainer)
        .WithEnumAttribute(Fields.HorizontalAlign, node.Align)
        .WithEnumAttribute(Fields.Type, node.Role)
        .WithIntAttribute("indent", node.Indent)
        .WithBooleanAttribute(Fields.Skip, node.Skip)
        .WithChild(serializer.Serialize(node.Child));


    public WemlParagraphContainer? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.ParagraphContainer)
        {
            return null;
        }

        bool skip = htmlNode[Fields.Skip]?.GetValue<bool>() == true;
        int indent = htmlNode["indent"]?.GetValue<int>() ?? 1;
        WemlHorizontalAlign? align = EnumsNET.Enums.TryParse(
            htmlNode[Fields.HorizontalAlign]?.GetValue<string>(),
            true, out WemlHorizontalAlign alignTmp,
            EnumFormat.Description)
            ? alignTmp
            : null;
        WemlParagraphRole? role = EnumsNET.Enums.TryParse(
            htmlNode[Fields.Type]?.GetValue<string>(),
            true, out WemlParagraphRole roleTmp,
            EnumFormat.Description)
            ? roleTmp
            : null;
        return new WemlParagraphContainer(htmlNode.GetChildren(deserializer).EnsureNodeTypes<WemlTextBlockElement>(htmlNode).Single())
        {
            Skip = skip, Indent = indent, Align = align, Role = role
        };
    }
}