namespace Egw.PubManagement.Application.Messaging.Folders;

/// <summary>
/// Delete a folder by ID
/// </summary>
public class DeleteFolderInput : IApplicationCommand
{
    /// <summary></summary>
    public required int Id { get; init; }

}