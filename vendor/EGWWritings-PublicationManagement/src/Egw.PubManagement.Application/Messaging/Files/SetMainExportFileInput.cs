using Egw.PubManagement.Persistence.Enums;

namespace Egw.PubManagement.Application.Messaging.Files;

/// <summary>
/// Set main export file
/// </summary>
public class SetMainExportFileInput : IApplicationCommand
{
    /// <summary>
    /// File ID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Publication ID
    /// </summary>
    public required int PublicationId { get; init; }

    /// <summary>
    /// Export type
    /// </summary>
    public required ExportTypeEnum Type { get; init; }
}