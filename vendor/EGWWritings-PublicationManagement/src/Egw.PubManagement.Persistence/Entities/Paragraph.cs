using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Paragraph
/// </summary>
public sealed class Paragraph : ITimeStampedEntity
{
    #region References

    /// <summary>
    /// Paragraph ID (unique key)
    /// </summary>
    public ParaId ParaId { get; private set; }

    /// <summary>
    /// Publication id
    /// </summary>
    public int PublicationId { get; private set; }

    /// <summary>
    /// Publication navigation property
    /// </summary>
    public Publication Publication { get; private set; } = null!;

    /// <summary>
    /// Paragraph ID (unique in book)
    /// </summary>
    public int ParagraphId { get; private set; }

    #endregion

    /// <summary>
    /// Paragraph order (unique in book)
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Is referenced
    /// </summary>
    public bool IsReferenced { get; set; } = true;

    /// <summary>
    /// Heading level: null for pages, 0 for paragraphs, 1 for headings, 2 for subheadings, etc.
    /// </summary>
    public int? HeadingLevel { get; set; }

    /// <summary>
    /// Paragraph content
    /// </summary>
    public string Content { get; set; } = "";

    /// <summary>
    /// Paragraph metadata navigation property
    /// </summary>
    public ParagraphMetadata? Metadata { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }


    /// <summary>
    /// Creates a new paragraph
    /// </summary>
    /// <param name="paraId">Paragraph ID</param>
    /// <param name="createdAt">Date of creation</param>
    public Paragraph(ParaId paraId, DateTimeOffset createdAt)
    {
        ParaId = paraId;
        ParagraphId = paraId.ElementId;
        PublicationId = paraId.PublicationId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Change paragraph content
    /// </summary>
    /// <param name="content">New paragraph content</param>
    /// <param name="headingLevel">New heading level</param>
    /// <param name="isReferenced">New is referenced</param>
    /// <param name="updatedAt">Date of update</param>
    public Paragraph ChangeContent(String content, int? headingLevel, bool isReferenced, DateTimeOffset updatedAt)
    {
        Content = content;
        HeadingLevel = headingLevel;
        IsReferenced = isReferenced;
        UpdatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Change paragraph order
    /// </summary>
    /// <param name="order">New paragraph order</param>
    /// <param name="updatedAt">Date of update</param>
    public Paragraph ChangeOrder(int order, DateTimeOffset updatedAt)
    {
        Order = order;
        UpdatedAt = updatedAt;
        return this;
    }
}