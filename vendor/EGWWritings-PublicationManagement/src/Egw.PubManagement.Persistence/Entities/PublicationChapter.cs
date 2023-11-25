using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Chapter header
/// </summary>
public class PublicationChapter : ITimeStampedEntity
{
    /// <summary>
    /// Paragraph ID for a chapter
    /// </summary>
    public ParaId ParaId { get; private set; }

    /// <summary>
    /// Publication id
    /// </summary>
    public int PublicationId { get; private set; }

    /// <summary>
    /// Paragraph navigation property
    /// </summary>
    public Paragraph Paragraph { get; set; } = null!;

    /// <summary>
    /// End Para Id
    /// </summary>
    public ParaId EndParaId { get; private set; }

    /// <summary>
    /// Paragraph navigation property
    /// </summary>
    public Paragraph EndParagraph { get; set; } = null!;

    /// <summary>
    /// End Para Id
    /// </summary>
    public ParaId ContentEndParaId { get; private set; }

    /// <summary>
    /// Paragraph navigation property
    /// </summary>
    public Paragraph ContentEndParagraph { get; set; } = null!;

    /// <summary>
    /// Chapter level
    /// </summary>
    public int Level { get; private set; }

    /// <summary>
    /// Chapter identifier
    /// </summary>
    public ParaId ChapterId { get; private set; }

    /// <summary>
    /// Chapter order
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Chapter end order
    /// </summary>
    public int EndOrder { get; private set; }

    /// <summary>
    /// Chapter end order
    /// </summary>
    public int ContentEndOrder { get; private set; }

    /// <summary>
    /// Chapter end order
    /// </summary>
    public int ChildCount => ContentEndOrder - Order;

    /// <summary>
    /// Chapter title
    /// </summary>
    public string Title { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new chapter header entry
    /// </summary>
    public PublicationChapter(
        ParaId paraId,
        int order,
        ParaId endParaId,
        int endOrder,
        ParaId contentEndParaId,
        int contentEndOrder,
        int level,
        ParaId chapterId,
        string title,
        DateTimeOffset createdAt)
    {
        ParaId = paraId;
        Order = order;
        EndParaId = endParaId;
        EndOrder = endOrder;
        ContentEndParaId = contentEndParaId;
        ContentEndOrder = contentEndOrder;

        Level = level;
        PublicationId = paraId.PublicationId;
        ChapterId = chapterId;

        Title = title;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }
}