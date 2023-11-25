namespace Egw.PubManagement.Application.Messaging.Folders;

/// <summary>
/// Changes folder type
/// </summary>
public class ChangeFolderTypeInput : IApplicationCommand
{
    /// <summary>Folder ID</summary>
    public required int Id { get; init; }
    /// <summary>New folder Type</summary>
    public required string Type { get; init; }
}