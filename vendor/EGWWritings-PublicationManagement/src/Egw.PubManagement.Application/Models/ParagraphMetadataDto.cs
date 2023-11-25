// using WhiteEstate.Doc.Persistence.Entities;
// using WhiteEstate.Doc.Persistence.Entities.Metadata;
// using WhiteEstate.DocFormat;
//
// namespace WhiteEstate.Management.Application.Models;
//
// /// <summary>
// /// Paragraph metadata dto
// /// <see cref="ParagraphMetadata"/> 
// /// </summary>
// public class ParagraphMetadataDto : ITimeStampedEntity
// {
//     /// <summary>
//     /// Paragraph ID
//     /// </summary>
//     [IsProjected]
//     public ParaId ParaId { get; init; }
//
//     /// <summary>
//     /// Publication id
//     /// </summary>
//     public int PublicationId { get; init; }
//
//     /// <summary>
//     /// Pagination section
//     /// </summary>
//     public PaginationMetaData? Pagination { get; init; }
//
//     /// <summary>
//     /// Paragraph date
//     /// </summary>
//     public DateOnly? Date { get; init; }
//
//     /// <summary>
//     /// End date for paragraph
//     /// </summary>
//     public DateOnly? EndDate { get; init; }
//
//     /// <summary>
//     /// Bible metadata
//     /// </summary>
//     public BibleMetadata? BibleMetadata { get; init; }
//
//     /// <summary>
//     /// Short reference code
//     /// </summary>
//     public string? RefCodeShort { get; init; }
//
//     /// <summary>
//     /// Long reference code
//     /// </summary>
//     public string? RefCodeLong { get; init; }
//
//     /// <summary>
//     /// Has manuscript metadata
//     /// </summary>
//     public LtMsMetadata? LtMsMetadata { get; init; }
//
//     /// <inheritdoc />
//     public DateTimeOffset CreatedAt { get; init; }
//
//     /// <inheritdoc />
//     public DateTimeOffset UpdatedAt { get; init; }
// }