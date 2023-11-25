using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Preview.Models.Internal;

/// <summary>
/// Internal temporary paragraph DTO
/// </summary>
public class ParagraphTemporaryDto
{
    /// <summary> Paragraph id </summary>
    public required ParaId ParaId { get; init; }
    /// <summary> Raw content </summary>
    public required string Content { get; init; }
    /// <summary> Order </summary>
    public required int Order { get; init; }
    /// <summary> Publication type </summary>
    public required PublicationType PublicationType { get; init; }
    /// <summary> Publication code </summary>
    public required string PublicationCode { get; init; }
    /// <summary> Publication title </summary>
    public required string PublicationTitle { get; init; }
    /// <summary> Metadata </summary>
    public ParagraphMetadata? Metadata { get; init; }
}