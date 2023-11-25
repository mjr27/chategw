namespace Egw.PubManagement.EpubExport.Types;

/// <summary>
/// Footnote info
/// </summary>
public class EpubFootNoteInfo
{
    /// <summary> Id </summary>
    public int Id { get; set; }

    /// <summary> Header </summary>
    public string Header { get; set; } = "";

    /// <summary> Class </summary>
    public string Class { get; set; } = "footnote";

    /// <summary> Content </summary>
    public string Content { get; set; } = "";
}