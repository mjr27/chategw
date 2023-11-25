using System.Text.Json.Nodes;
namespace WhiteEstate.DocFormat.Json;

/// <summary>
/// WeML => HTML serializer
/// </summary>
public interface IWemlJsonDeserializer
{
    /// <summary>
    /// Deserializes custom node
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    IWemlNode Deserialize(JsonObject node);

    /// <summary>
    /// Serializes whole document
    /// </summary>
    /// <param name="document">Document to deserialize</param>
    /// <returns></returns>
    WemlDocument DeserializeDocument(JsonObject document);
}