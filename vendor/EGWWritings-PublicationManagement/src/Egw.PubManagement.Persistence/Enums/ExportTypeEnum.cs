using System.ComponentModel;
namespace Egw.PubManagement.Persistence.Enums;

/// <summary>
/// Export files enum
/// </summary>
public enum ExportTypeEnum
{
    /// <summary>
    /// EPub file
    /// </summary>
    [Description("epub")] Epub,

    /// <summary>
    /// Kindle Mobi file
    /// </summary>
    [Description("mobi")] Mobi,

    /// <summary>
    /// Mp3 file
    /// </summary>
    [Description("mp3")] Mp3,

    /// <summary>
    /// PDF File
    /// </summary>
    [Description("pdf")] Pdf
}