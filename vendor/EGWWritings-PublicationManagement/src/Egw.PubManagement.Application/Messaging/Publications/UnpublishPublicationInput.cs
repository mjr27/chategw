namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary>
/// Unpublishes a publication
/// </summary>
public record UnpublishPublicationInput : IApplicationCommand
{
    /// <summary>
    /// Publication id
    /// </summary>
    public required int PublicationId { get; init; }
}