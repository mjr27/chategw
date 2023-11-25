namespace Egw.PubManagement.Application.Messaging.Archive;

/// <summary>
/// Delete publication from archive
/// </summary>
public class DeleteEntryFromArchiveInput : IApplicationCommand
{
    /// <summary>
    /// Archive entity ID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Publication ID
    /// </summary>
    public required int PublicationId { get; init; }
}