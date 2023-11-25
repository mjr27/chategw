namespace Egw.PubManagement.Application.Messaging.Covers;

/// <summary> Create cover type input </summary>
public class CreateCoverTypeInput : IApplicationCommand
{
    /// <summary> Cover type code </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Cover type description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Publication required int MinWidth
    /// </summary>
    public required int MinWidth { get; init; }

    /// <summary>
    /// Publication required int MinHeight
    /// </summary>
    public required int MinHeight { get; init; }
}