using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json;

/// <inheritdoc />
public class WemlJsonSerializer : IWemlJsonSerializer
{
    // private readonly IHtmlDocument _document;

    /// <summary>
    /// Creates a new serializer
    /// </summary>
    public WemlJsonSerializer()
    {
        _repository = new WemlSerializerRepository();
    }

    /// <inheritdoc />
    public JsonObject Serialize(WemlDocument document)
    {
        var jsonDocument = new JsonObject();
        var jsonMeta = new JsonObject();
        var body = new JsonArray();
        jsonDocument["meta"] = jsonMeta;
        jsonDocument["body"] = body;

        jsonMeta[Fields.Id] = document.Header.Id;
        jsonMeta[Fields.Meta.Code] = document.Header.Code;
        jsonMeta[Fields.Meta.Language] = document.Header.Language;
        jsonMeta[Fields.Meta.Title] = document.Header.Title;
        jsonMeta.WithSubtype(document.Header.Type);
        foreach (WemlParagraph child in document.Children)
        {
            JsonObject childElement = Serialize(child.Element);
            childElement[Fields.Id] = child.Id;
            body.Add(childElement);
        }

        return jsonDocument;
    }

    /// <inheritdoc />
    public JsonObject Serialize(IWemlNode root) => _repository.Serialize(root, this);


    private readonly WemlSerializerRepository _repository;
}