namespace Egw.PubManagement.EpubExport.Types;

/// <summary> Epub chapter tree item </summary>
public class EpubTreeItem
{
    /// <summary> Chapter </summary>
    public required EpubChapter Chapter { get; init; }
    /// <summary> Children </summary>
    public List<EpubTreeItem> Children { get; init; } = new();
}