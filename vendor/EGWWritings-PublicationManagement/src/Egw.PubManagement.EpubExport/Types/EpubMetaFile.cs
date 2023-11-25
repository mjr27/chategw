namespace Egw.PubManagement.EpubExport.Types;

/// <summary> META file </summary>
public class EpubMetaFile
{
    /// <summary> File </summary>
    public string File { get; set; } = "";

    /// <summary> Name </summary>
    public string Name { get; set; } = "";

    /// <summary> Title </summary>
    public string Title { get; set; } = "";

    /// <summary> Order </summary>
    public int Order { get; set; }
}