using System.ComponentModel;
namespace Egw.PubManagement.Persistence.Enums;

/// <summary>
/// Global option
/// </summary>
public enum GlobalOptionTypeEnum
{
    /// <summary>
    /// Link cache
    /// </summary>
    [Description("link-cache")]
    LinkCache,
    /// <summary>
    /// Mp3 cache
    /// </summary>
    [Description("mp3-cache")]
    Mp3Cache
}