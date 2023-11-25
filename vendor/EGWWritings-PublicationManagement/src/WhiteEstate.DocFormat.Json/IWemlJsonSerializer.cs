using System.Text.Json.Nodes;
namespace WhiteEstate.DocFormat.Json;

/// <summary>
/// WeML => HTML serializer
/// </summary>
public interface IWemlJsonSerializer
{
    /// <summary>
    /// Serializes weml node to HTML
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    JsonObject Serialize(IWemlNode root);

    /// <summary>
    /// Serializes whole document
    /// </summary>
    /// <param name="document">Document to serialize</param>
    /// <returns></returns>
    JsonObject Serialize(WemlDocument document);
}