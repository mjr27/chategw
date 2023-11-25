namespace Egw.PubManagement.Application.Messaging.Archive;

/// <summary>
/// Save WEML to archive
/// </summary>
public class SavePublicationToArchiveInput : IApplicationCommand
{
    /// <summary>
    /// Publication ID
    /// </summary>
    public required int PublicationId { get; init; }
}