namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Cover type
/// </summary>
public class CoverType
{
    /// <summary>
    /// Cover type code
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Minimum width of the cover type. If null, minimal width is not specified.
    /// </summary>
    public required int MinWidth { get; init; }

    /// <summary>
    /// Minimum width of the cover type. If null, minimal width is not specified.
    /// </summary>
    public required int MinHeight { get; init; }

    /// <summary>
    /// Cover type description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Covers navigation property
    /// </summary>
    public List<Cover> Covers { get; init; } = new();
}