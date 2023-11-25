namespace Egw.PubManagement.LatexExport;

/// <summary>
/// Latex publication options
/// </summary>
public sealed class LatexPublicationOptions
{
    /// <summary>
    /// Paragraph before which to insert TOC
    /// </summary>
    public int? InsertTocBefore { get; set; }

    /// <summary>
    /// Level for end notes
    /// </summary>
    public int EndNoteLevel { get; set; } = 1;
}