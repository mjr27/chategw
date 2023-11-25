using System.Text.Json;
using System.Text.Json.Serialization;

using Egw.PubManagement.Application.Models.Internal;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application;

/// <summary>
/// Converts a <see cref="ParaId"/> to and from JSON.
/// </summary>
public class Mp3FileNameNullableConverter : JsonConverter<Mp3FileName?>
{
    /// <inheritdoc />
    public override Mp3FileName? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Mp3FileName.Parse(reader.GetString());
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Mp3FileName? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}