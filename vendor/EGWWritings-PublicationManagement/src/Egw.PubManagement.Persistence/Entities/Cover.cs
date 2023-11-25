using Egw.PubManagement.Persistence.Enums;
namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Cover type
/// </summary>
public class Cover : ITimeStampedEntity
{
    /// <summary> Unique identifier of the cover </summary>
    public required Guid Id { get; init; }

    /// <summary>  Cover type code </summary>
    public required string TypeId { get; init; }

    /// <summary> Is main cover for a publication/type </summary>
    public bool IsMain { get; set; }

    /// <summary> Cover type navigation property </summary>
    public CoverType Type { get; init; } = default!;

    /// <summary> Publication identifier </summary>
    public required int PublicationId { get; init; }
    /// <summary> Publication navigation property </summary>
    public Publication Publication { get; init; } = default!;

    /// <summary> Original width </summary>
    public required int Width { get; init; }
    
    /// <summary> Original height </summary>
    public required int Height { get; init; }

    /// <summary> Original file size </summary>
    public required long Size { get; init; }

    /// <summary> Media format </summary>
    public required MediaFormatEnum Format { get; init; }

    /// <summary> Relative URI to the cover </summary>
    public required Uri Uri { get; init; }

    /// <inheritdoc />
    public required DateTimeOffset CreatedAt { get; init; }

    /// <inheritdoc />
    public required DateTimeOffset UpdatedAt { get; set; }
    
}