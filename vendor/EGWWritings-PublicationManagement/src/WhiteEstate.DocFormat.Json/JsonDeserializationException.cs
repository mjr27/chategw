using System.Text.Json.Nodes;
namespace WhiteEstate.DocFormat.Json;

/// <summary>
/// Deserialization exception
/// </summary>
public class JsonDeserializationException : Exception
{
    /// <summary>
    /// Node where error occured
    /// </summary>
    public JsonObject? Element { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public JsonDeserializationException(string message, JsonObject? element)
        : base(message)
    {
        Element = element;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return base.ToString() + "\n\n" + Element?.ToJsonString();
    }
}