using System.Text.Json;
using System.Text.Json.Serialization;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application;

/// <summary>
/// Converts a <see cref="ParaId"/> to and from JSON.
/// </summary>
public class ParaIdConverter : JsonConverter<ParaId>
{
    /// <inheritdoc />
    public override ParaId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ParaId.Parse(reader.GetString());
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ParaId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}