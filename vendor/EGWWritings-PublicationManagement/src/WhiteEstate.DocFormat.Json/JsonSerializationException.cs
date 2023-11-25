namespace WhiteEstate.DocFormat.Json;

/// <summary>
/// Deserialization exception
/// </summary>
public class JsonSerializationException : Exception
{
    /// <summary>
    /// Weml element type
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public JsonSerializationException(string message, Type type) : base(message)
    {
        this.Type = type;
    }
}