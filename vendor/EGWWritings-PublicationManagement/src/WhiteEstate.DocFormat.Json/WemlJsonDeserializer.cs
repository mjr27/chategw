using System.Text.Json.Nodes;

using EnumsNET;

using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Json.Internal;
namespace WhiteEstate.DocFormat.Json;

/// <inheritdoc />
public class WemlJsonDeserializer : IWemlJsonDeserializer
{
    /// <summary>
    /// Creates a new serializer
    /// </summary>
    public WemlJsonDeserializer()
    {
        _repository = new WemlSerializerRepository();
    }

    private readonly WemlSerializerRepository _repository;

    /// <inheritdoc />
    public IWemlNode Deserialize(JsonObject node) => _repository.Deserialize(node, this);

    /// <inheritdoc />
    public WemlDocument DeserializeDocument(JsonObject document)
    {
        WemlDocumentHeader header = FetchHeader(document);
        var result = new WemlDocument(header);
        foreach (JsonObject child in document["body"]!.AsArray().OfType<JsonObject>())
        {
            int elementId = child[Fields.Id]!.GetValue<int>();
            IWemlNode childNode = Deserialize(child);
            if (childNode is not IWemlContainerElement container)
            {
                throw new JsonDeserializationException("Paragraph container may contain only container elements", child);
            }

            result.Children.Add(new WemlParagraph(elementId, container));
        }

        return result;
    }

    private static WemlDocumentHeader FetchHeader(JsonObject document)
    {
        return new WemlDocumentHeader(
            FetchMeta<int>(document, Fields.Id),
            EnumsNET.Enums.Parse<PublicationType>(FetchMeta<string>(document, Fields.Type), true, EnumFormat.Description),
            FetchMeta<string>(document, Fields.Meta.Language),
            FetchMeta<string>(document, Fields.Meta.Code)
        ) { Title = FetchMeta<string>(document, Fields.Meta.Title) };
    }

    private static T FetchMeta<T>(JsonNode root, string name)
    {
        JsonNode? value = root["meta"]?[name];
        if (value is null)
        {
            throw new JsonDeserializationException($"Cannot find meta tag {name}", null);
        }

        T result = value.GetValue<T>();
        if (result is null)
        {
            throw new JsonDeserializationException($"Cannot find meta tag {name}", null);
        }

        return result;
    }
}