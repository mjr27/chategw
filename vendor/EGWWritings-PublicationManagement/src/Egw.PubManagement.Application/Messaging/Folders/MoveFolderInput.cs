namespace Egw.PubManagement.Application.Messaging.Folders;

/// <summary>
/// Move Folder Command
/// </summary>
public class MoveFolderInput : IApplicationCommand
{
    /// <summary>
    /// Folder Id
    /// </summary>
    public required int Id { get; set; }
    /// <summary>
    /// New Parent Folder Id
    /// </summary>
    public int? NewParent { get; set; }
    /// <summary>
    /// New Position
    /// </summary>
    public int? NewPosition { get; set; }
}