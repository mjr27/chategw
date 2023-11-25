using System.Text.Json.Nodes;

using EnumsNET;

using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json.Serializers.Inline;

/// <inheritdoc />
public class NoteSerializer : IWemlJsonElementSerializer<WemlNoteNode>
{
    /// <inheritdoc />
    public JsonObject Serialize(IWemlJsonSerializer serializer, WemlNoteNode node) => new JsonObject()
        .WithJsonNodeType(JsonNodeType.Note)
        .WithSubtype(node.Type)
        .WithChildren(serializer, node.Reference, Fields.NoteReference)
        .WithChildren(serializer, node.Content);

    /// <inheritdoc />
    public WemlNoteNode? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode)
    {
        if (htmlNode.GetJsonNodeType() is not JsonNodeType.Note)
        {
            return null;
        }

        if (!EnumsNET.Enums.TryParse(
                htmlNode[Fields.Type]?.GetValue<string>(),
                true, out WemlNoteType type,
                EnumFormat.Description))
        {
            throw new JsonDeserializationException("Invalid note type", htmlNode);
        }

        return new WemlNoteNode(type,
            htmlNode.GetChildren(deserializer, Fields.NoteReference).EnsureNodeTypes<IWemlInlineNode>(htmlNode),
            htmlNode.GetChildren(deserializer).EnsureNodeTypes<IWemlBlockNode>(htmlNode)
        );
    }
}