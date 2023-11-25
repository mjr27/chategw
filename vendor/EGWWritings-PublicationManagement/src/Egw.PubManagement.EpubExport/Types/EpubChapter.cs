using WhiteEstate.DocFormat;

namespace Egw.PubManagement.EpubExport.Types;

/// <summary>
/// Prepared chapter
/// </summary>
public class EpubChapter
{
    /// <summary> File </summary>
    public string File { get; set; } = "";

    /// <summary> Id </summary>
    public string Id { get; set; } = "";

    /// <summary> Title </summary>
    public string Title { get; set; } = "";

    /// <summary> File number </summary>
    public int FileNumber { get; set; }

    /// <summary> Formatted file number </summary>
    public string FileNumberFormatted { get; set; } = "0000";

    /// <summary> Order </summary>
    public int Order { get; set; }

    /// <summary> End order </summary>
    public int EndOrder { get; set; }

    /// <summary> Level </summary>
    public int Level { get; set; }

    /// <summary> Publication id </summary>
    public int PublicationId { get; set; }

    /// <summary> List of pages </summary>
    public List<EpubPageInfo> Pages { get; set; } = new();

    /// <summary> Paragraph id </summary>
    public ParaId ParaId { get; set; }

    /// <summary> List of footnotes </summary>
    public List<EpubFootNoteInfo> Footnotes { get; set; } = new();

    /// <summary> Play order </summary>
    public int PlayOrder { get; set; }

    /// <summary> List of children </summary>
    public List<EpubChapter> Children { get; set; } = new();
}