namespace Egw.PubManagement.Application.Messaging.Files;

/// <summary> Delete export file </summary>
public class DeleteExportFileInput : IApplicationCommand
{
    /// <summary> File ID </summary>
    public required Guid Id { get; init; }
}