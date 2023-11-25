namespace Egw.PubManagement.Application.Messaging.Mp3;

/// <inheritdoc />
public class UpdateMp3ManifestInput : IApplicationCommand
{
    /// <summary>
    /// Publication Id
    /// </summary>
    public required int PublicationId { get; init; }

    /// <summary>
    /// Update\Replace manifest
    /// </summary>
    public bool ReplaceManifest { get; init; } = false;
}