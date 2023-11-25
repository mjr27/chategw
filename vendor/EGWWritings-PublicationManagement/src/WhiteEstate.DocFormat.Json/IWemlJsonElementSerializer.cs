using System.Text.Json.Nodes;
namespace WhiteEstate.DocFormat.Json;

/// <summary>
/// Node serializer
/// </summary>
/// <typeparam name="TNode"></typeparam>
public interface IWemlJsonElementSerializer<TNode> where TNode : IWemlNode
{
    /// <summary>
    /// Serializes node
    /// </summary>
    /// <param name="serializer">serializer</param>
    /// <param name="node">Node to enrich</param>
    /// <returns></returns>
    JsonObject Serialize(IWemlJsonSerializer serializer, TNode node);

    /// <summary>
    /// Tries deserializing node
    /// </summary>
    /// <param name="deserializer">Deserializer</param>
    /// <param name="htmlNode">Html node</param>
    /// <returns></returns>
    TNode? Deserialize(IWemlJsonDeserializer deserializer, JsonObject htmlNode);
}