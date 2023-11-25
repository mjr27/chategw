using Egw.PubManagement.Persistence.Enums;

using HotChocolate.Types;

namespace Egw.PubManagement.Application.Messaging.Files;

/// <summary>
/// Input for cover upload
/// </summary>
public class UploadExportInput : IApplicationCommand
{
    /// <summary>
    /// Cover ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Cover type
    /// </summary>
    public required ExportTypeEnum Type { get; init; }

    /// <summary>
    /// Publication ID
    /// </summary>
    public required int PublicationId { get; init; }

    /// <summary>
    /// Uploaded file
    /// </summary>
    public required IFile File { get; init; }
}