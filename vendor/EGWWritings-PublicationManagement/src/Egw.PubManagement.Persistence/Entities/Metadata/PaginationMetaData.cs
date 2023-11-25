namespace Egw.PubManagement.Persistence.Entities.Metadata;

/// <summary>
/// Pagination metadata
/// </summary>
public class PaginationMetaData
{
    /// <summary>
    /// Pagination section
    /// </summary>
    public string Section { get; init; } = "";

    /// <summary>
    /// Pagination paragraph
    /// </summary>
    public int Paragraph { get; init; }
}