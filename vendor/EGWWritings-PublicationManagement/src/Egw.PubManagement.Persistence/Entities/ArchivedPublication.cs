namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Archived publication
/// </summary>
public class ArchivedPublication
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; private set; }


    /// <summary>
    /// Publication identifier
    /// </summary>
    public int PublicationId { get; private set; }


    /// <summary>
    /// Text of the archive
    /// </summary>
    public string ArchiveText { get; private set; }

    /// <summary>
    /// Archive hash
    /// </summary>
    public string Hash { get; private set; }


    /// <summary>
    /// Archived publication version
    /// </summary>
    public DateTimeOffset ArchivedAt { get; private set; }

    /// <summary>
    /// Archived entity is deleted
    /// </summary>
    public DateTimeOffset? DeletedAt { get; init; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="publicationId"></param>
    /// <param name="archiveText"></param>
    /// <param name="hash"></param>
    /// <param name="archivedAt"></param>
    public ArchivedPublication(int publicationId, string archiveText, string hash, DateTimeOffset archivedAt)
    {
        Id = Guid.NewGuid();
        PublicationId = publicationId;
        ArchiveText = archiveText;
        Hash = hash;
        ArchivedAt = archivedAt;
    }
}