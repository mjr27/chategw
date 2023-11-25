namespace Egw.PubManagement.Persistence.Entities.Metadata;

/// <summary>
/// Metadata for letters/manuscripts
/// </summary>
public class LtMsMetadata
{
    /// <summary>
    /// Manuscript addressee
    /// </summary>
    public string? Addressee { get; set; }

    /// <summary>
    /// Manuscript title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Manuscript place
    /// </summary>
    public string? Place { get; set; }
}