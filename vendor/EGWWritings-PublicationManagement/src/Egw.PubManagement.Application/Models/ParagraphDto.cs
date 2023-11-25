using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Entities.Metadata;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Models;

/// <summary>
/// Paragraph DTO
/// </summary>
public class ParagraphDto : ITimeStampedEntity, IProjectedDto<Paragraph>
{
    /// <summary>
    /// Paragraph ID (unique key)
    /// </summary>
    public ParaId ParaId { get; init; }

    /// <summary>
    /// Publication id
    /// </summary>
    public int PublicationId { get; init; }

    /// <summary>
    /// Paragraph ID (unique in book)
    /// </summary>
    public int ParagraphId { get; init; }

    /// <summary>
    /// Paragraph order (unique in book)
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Is referenced
    /// </summary>
    public bool IsReferenced { get; init; } = true;

    /// <summary>
    /// Heading level: null for pages, 0 for paragraphs, 1 for headings, 2 for subheadings, etc.
    /// </summary>
    public int? HeadingLevel { get; init; }

    /// <summary>
    /// Paragraph content
    /// </summary>
    public string Content { get; init; } = "";

    /// <summary>
    /// Paragraph date
    /// </summary>
    public DateOnly? Date { get; init; }

    /// <summary>
    /// End date for paragraph
    /// </summary>
    public DateOnly? EndDate { get; init; }

    /// <summary>
    /// Pagination section
    /// </summary>
    public PaginationMetaData? Pagination { get; init; }

    /// <summary>
    /// Short reference code
    /// </summary>
    public string? RefCodeShort { get; init; }

    /// <summary>
    /// Long reference code
    /// </summary>
    public string? RefCodeLong { get; init; }

    /// <summary>
    /// Bible metadata
    /// </summary>
    public BibleMetadata? BibleMetadata { get; init; }

    /// <summary>
    /// Has manuscript metadata
    /// </summary>
    public LtMsMetadata? LtMsMetadata { get; init; }


    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; init; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; init; }
}