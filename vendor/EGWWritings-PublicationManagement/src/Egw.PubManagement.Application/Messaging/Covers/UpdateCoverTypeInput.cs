namespace Egw.PubManagement.Application.Messaging.Covers;

/// <summary> Update cover type input </summary>
public class UpdateCoverTypeInput : IApplicationCommand
{
    /// <summary> Cover type code </summary>
    public required string Code { get; init; }
    /// <summary>
    /// Cover type description
    /// </summary>
    public string? Description { get; init; }
}