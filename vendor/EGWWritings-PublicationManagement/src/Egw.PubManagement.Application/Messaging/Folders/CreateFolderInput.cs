namespace Egw.PubManagement.Application.Messaging.Folders;

/// <summary>
/// Creates a new folder at the end of specified parent folder
/// </summary>
public class CreateFolderInput : IApplicationCommand
{
    /// <summary>Parent folder ID</summary>
    public required int ParentId { get; init; }
    
    /// <summary>Title</summary>
    public required string Title { get; init; }
    
    /// <summary>Type</summary>
    public required string Type { get; init; }
}