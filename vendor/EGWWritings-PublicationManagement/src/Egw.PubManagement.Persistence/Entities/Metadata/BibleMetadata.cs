namespace Egw.PubManagement.Persistence.Entities.Metadata;

/// <summary>
/// Bible metadata
/// </summary>
public class BibleMetadata
{
    /// <summary>
    /// Bible book
    /// </summary>
    public string Book { get; init; } = "";

    /// <summary>
    /// Bible chapter
    /// </summary>
    public int Chapter { get; init; }

    /// <summary>
    /// Bible verse
    /// </summary>
    public IReadOnlyCollection<int> Verses { get; init; } = new List<int>();
}