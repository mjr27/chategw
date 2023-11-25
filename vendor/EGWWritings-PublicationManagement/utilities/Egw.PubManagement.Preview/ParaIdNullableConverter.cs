using System.Text.Json;
using System.Text.Json.Serialization;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Preview;

internal class ParaIdNullableConverter : JsonConverter<ParaId?>
{
    public override ParaId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ParaId.TryParse(reader.GetString(), out ParaId paraId) ? paraId : null;
    }

    public override void Write(Utf8JsonWriter writer, ParaId? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}