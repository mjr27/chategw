using MediatR;

namespace Egw.PubManagement.Application.Messaging.Folders;

/// <summary>
/// Rename a folder
/// </summary>
public class RenameFolderInput : IRequest
{
    /// <summary>Folder ID</summary>
    public required int Id { get; init; }
    /// <summary>New Title</summary>
    public required string Title { get; init; }
}