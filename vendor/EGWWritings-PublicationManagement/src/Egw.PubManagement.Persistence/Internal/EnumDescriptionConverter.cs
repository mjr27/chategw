using EnumsNET;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace Egw.PubManagement.Persistence.Internal;

/// <summary> 
/// Database converter for enums
/// </summary>
internal class EnumDescriptionConverter<T> : ValueConverter<T, string> where T : struct, Enum
{
    /// <inheritdoc />
    public EnumDescriptionConverter() : base(
        value => SerializeString(value),
        s => ParseString(s))
    {
    }

    private static string SerializeString(T value)
    {
        return value.AsString(EnumFormat.Description)!;
    }

    private static T ParseString(string s)
    {
        return EnumsNET.Enums.TryParse(s, true, out T value, EnumFormat.Description)
            ? value
            : throw new ArgumentException($"Invalid enum value: {s}", nameof(s));
    }
}