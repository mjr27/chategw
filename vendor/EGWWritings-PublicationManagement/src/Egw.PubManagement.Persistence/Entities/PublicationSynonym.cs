namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Publications series
/// </summary>
public class PublicationSynonym : ITimeStampedEntity
{

    /// <summary>
    /// Synonym ID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// Publication ID
    /// </summary>
    public int PublicationId { get; private set; }

    /// <summary>
    /// Element ID
    /// </summary>
    public int? ElementId { get; private set; }

    /// <summary>
    /// Publication navigation property
    /// </summary>
    public Publication Publication { get; private set; } = null!;

    /// <summary>
    /// Synonym
    /// </summary>
    public string Synonym { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new series
    /// </summary>
    /// <param name="publicationId">Publication Id</param>
    /// <param name="elementId">Element Id</param>
    /// <param name="synonym">Synonym</param>
    /// <param name="createdAt"></param>
    public PublicationSynonym(
        int publicationId,
        int? elementId,
        string synonym,
        DateTimeOffset createdAt)
    {
        PublicationId = publicationId;
        ElementId = elementId;
        Synonym = synonym;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }
}