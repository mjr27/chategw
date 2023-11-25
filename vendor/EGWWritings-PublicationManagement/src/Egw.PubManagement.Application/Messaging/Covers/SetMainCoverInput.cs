namespace Egw.PubManagement.Application.Messaging.Covers;

/// <summary>
/// Sets main cover for a publication
/// </summary>
public class SetMainCoverInput : IApplicationCommand
{
    /// <summary> Cover identifier </summary>
    public Guid? Id { get; init; }
    /// <summary> Publication identifier </summary>
    public required string Type { get; init; }
    /// <summary> Publication identifier </summary>
    public required int PublicationId { get; init; }
}