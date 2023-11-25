using HotChocolate.Types;

namespace Egw.PubManagement.Application.Messaging.Archive;

/// <summary>
/// Restore saved publication from archive
/// </summary>
public class RestoreEntryFromArchiveInput : IApplicationCommand
{
    /// <summary>
    /// Archive entity ID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Publication ID
    /// </summary>
    public required int PublicationId { get; init; }

    /// <summary>
    /// Back up current version automatically
    /// </summary>
    [DefaultValue(true)]
    public bool AutoBackup { get; init; }
}