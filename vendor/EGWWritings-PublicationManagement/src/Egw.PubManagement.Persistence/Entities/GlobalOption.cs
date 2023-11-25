using System.Text.Json;

using Egw.PubManagement.Persistence.Enums;
namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Global configuration value
/// </summary>
public class GlobalOption : IDisposable
{
    /// <summary>
    /// Configuration key
    /// </summary>
    public GlobalOptionTypeEnum Key { get; private set; }
    /// <summary>
    /// Configuration value
    /// </summary>
    public JsonDocument Value { get; private set; }
    /// <summary> Date and time of last update </summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="updatedAt"></param>
    public GlobalOption(GlobalOptionTypeEnum key, JsonDocument value, DateTimeOffset updatedAt)
    {
        Key = key;
        Value = value;
        UpdatedAt = updatedAt;
    }

    /// <summary>
    /// Update configuration value
    /// </summary>
    /// <param name="newValue">New value</param>
    /// <param name="moment">Current moment</param>
    public void Update(JsonDocument newValue, DateTimeOffset moment)
    {
        Value = newValue;
        UpdatedAt = moment;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Value.Dispose();
    }
}