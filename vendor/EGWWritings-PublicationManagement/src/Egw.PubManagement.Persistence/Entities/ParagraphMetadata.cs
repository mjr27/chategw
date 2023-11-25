using Egw.PubManagement.Persistence.Entities.Metadata;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Paragraph metadata
/// </summary>
public sealed class ParagraphMetadata : ITimeStampedEntity
{
    /// <summary>
    /// Paragraph ID
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
    /// Pagination section
    /// </summary>
    public PaginationMetaData? Pagination { get; private set; }

    /// <summary>
    /// Paragraph date
    /// </summary>
    public DateOnly? Date { get; private set; }

    /// <summary>
    /// End date for paragraph
    /// </summary>
    public DateOnly? EndDate { get; private set; }

    /// <summary>
    /// Bible metadata
    /// </summary>
    public BibleMetadata? BibleMetadata { get; private set; }

    /// <summary>
    /// Has manuscript metadata
    /// </summary>
    public LtMsMetadata? LtMsMetadata { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new metadata for paragraph
    /// </summary>
    public ParagraphMetadata(ParaId paraId, DateTimeOffset createdAt)
    {
        ParaId = paraId;
        PublicationId = paraId.PublicationId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Adds pagination
    /// </summary> 
    /// <param name="section">Pagination section</param>
    /// <param name="paragraphNumber">Pagination paragraph number</param>
    /// <returns></returns>
    public ParagraphMetadata WithPagination(string section, int paragraphNumber)
    {
        Pagination = new PaginationMetaData { Section = section, Paragraph = paragraphNumber };
        return this;
    }

    /// <summary>
    /// Add date
    /// </summary>
    /// <param name="date">Date</param>
    /// <param name="endDate">End date</param>
    /// <returns></returns>
    public ParagraphMetadata WithDate(DateOnly? date, DateOnly? endDate)
    {
        Date = date;
        EndDate = endDate;
        return this;
    }

    /// <summary>
    /// Add bible references
    /// </summary>
    /// <param name="book">Bible book</param>
    /// <param name="chapter">Bible chapter</param>
    /// <param name="verses">Bible verses</param>
    /// <returns></returns>
    public ParagraphMetadata WithBible(string book, int chapter = 0, int[]? verses = null)
    {
        BibleMetadata = new BibleMetadata { Book = book, Chapter = chapter, Verses = verses ?? Array.Empty<int>() };
        return this;
    }

    /// <summary>
    /// Adds manuscript metadata
    /// </summary>
    /// <param name="addressee">Manuscript addressee</param>
    /// <param name="title">Manuscript title</param>
    /// <param name="place">Manuscript place</param>
    /// <returns></returns>
    public ParagraphMetadata WithManuscript(string? addressee, string? title, string? place)
    {
        if (addressee is null && title is null && place is null)
        {
            LtMsMetadata = null;
            return this;
        }

        LtMsMetadata = new LtMsMetadata { Addressee = addressee, Place = place, Title = title, };
        return this;
    }
}