using System.Data.Common;

namespace Egw.PubManagement.Storage;

internal static class DbConnectionStringBuilderExtensions
{
    public static string? GetValueOrDefault(this DbConnectionStringBuilder builder, string key,
        string? defaultValue) =>
        builder.TryGetValue(key, out object? value) ? (string?)value : defaultValue;

    public static string GetRequiredValue(this DbConnectionStringBuilder builder, string key)
    {
        return builder.TryGetValue(key, out object? value)
            ? (string)value
            : throw new ArgumentException("Missing connection string value", key);
    }
}