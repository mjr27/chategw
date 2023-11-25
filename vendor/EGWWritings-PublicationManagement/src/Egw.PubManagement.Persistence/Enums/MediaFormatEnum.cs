using System.ComponentModel;
namespace Egw.PubManagement.Persistence.Enums;

/// <summary>
/// Media format
/// </summary>
public enum MediaFormatEnum
{
    /// <summary>
    /// JPEG
    /// </summary>
    [Description("jpg")]
    Jpeg,
    /// <summary>
    /// PNG
    /// </summary>
    [Description("png")]
    Png
}