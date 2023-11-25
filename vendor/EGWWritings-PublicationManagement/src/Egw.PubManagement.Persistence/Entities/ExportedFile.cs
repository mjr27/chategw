using Egw.PubManagement.Persistence.Enums;

namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Exported file
/// </summary>
public class ExportedFile : ITimeStampedEntity
{
    /// <summary> Unique file identifier </summary>
    public Guid Id { get; private set; }

    /// <summary> Publication ID </summary>
    public int PublicationId { get; private set; }

    /// <summary> Is main export </summary>
    public bool IsMain { get; set; }

    /// <summary> Exported type enum </summary>
    public required ExportTypeEnum Type { get; init; }

    /// <summary> Original file size </summary>
    public required long Size { get; init; }

    /// <summary> Relative URI to the cover </summary>
    public required Uri Uri { get; init; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary> Publication navigation property </summary>
    public Publication Publication { get; private set; } = null!;

    /// <summary>
    /// Creates exported file
    /// </summary>
    /// <param name="id"></param>
    /// <param name="publicationId"></param>
    /// <param name="createdAt"></param>
    public ExportedFile(Guid id, int publicationId, DateTimeOffset createdAt)
    {
        Id = id;
        PublicationId = publicationId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }
}