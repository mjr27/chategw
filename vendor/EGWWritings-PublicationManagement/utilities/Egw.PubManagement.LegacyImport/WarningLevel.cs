using System.ComponentModel;
namespace Egw.PubManagement.LegacyImport;

/// <summary>
/// Warning level
/// </summary>
public enum WarningLevel
{
    /// <summary>
    /// Information. Minor information that may be ignored
    /// </summary>
    [Description("INF")] Information = 10,

    /// <summary>
    /// Warning. We failed to do some processing, but operation may continue
    /// </summary>
    [Description("WRN")] Warning = 20,

    /// <summary>
    /// Error. Some parsing was not successful. Unpredicted parsing results may happen
    /// </summary>
    [Description("ERR")] Error = 30
}