namespace Egw.PubManagement.EpubExport;

/// <summary>
/// EPub configuration
/// </summary>
public class EpubConfiguration
{
    /// <summary>
    /// Path to epub directory
    /// </summary>
    public string PathToEpubDirectory { get; set; } = "temp/";

    /// <summary>
    /// Leave raw files
    /// </summary>
    public bool LeaveRawFiles { get; set; } = false;
}