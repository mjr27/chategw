namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary> Sets publication max toc level </summary>
public class SetPublicationTocDepthInput : IApplicationCommand
{
    /// <summary> Publication ID </summary>
    public required int PublicationId { get; init; }
    /// <summary> Max toc level </summary>
    public int? TocDepth { get; init; }
}