namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary>
/// Publishes a publication
/// </summary>
public class PublishPublicationInput : IApplicationCommand
{
    /// <summary>Publication id</summary>
    public required int PublicationId { get; init; }
    /// <summary>Folder id</summary>
    public required int FolderId { get; init; }
}