namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary> Sets publication max toc level </summary>
public class MovePublicationInput : IApplicationCommand
{
    /// <summary> Publication ID </summary>
    public required int PublicationId { get; init; }
    /// <summary> New folder ID </summary>
    public required int FolderId { get; init; }
    /// <summary> New order to place publication into </summary>
    public int? Order { get; init; }
}