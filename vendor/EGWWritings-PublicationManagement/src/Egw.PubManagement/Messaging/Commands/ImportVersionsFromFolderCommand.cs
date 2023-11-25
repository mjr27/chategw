using MediatR;
namespace Egw.PubManagement.Messaging.Commands;

/// <summary>
/// Import all publications from a folder into repository
/// </summary>
/// <param name="Folder"></param>
public record ImportVersionsFromFolderCommand(string Folder) : IRequest
{
    /// <summary>
    /// Skips existing publications (publications that contain paragraphs)
    /// </summary>
    public bool SkipExisting { get; init; }
}