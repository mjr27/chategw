namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary>
/// Sets order for a publication
/// </summary>
public class SetPublicationOrderInput : IApplicationCommand
{
    /// <summary>Publication id</summary>
    public required int PublicationId { get; init; }
    /// <summary>New order</summary>
    public required float Order { get; init; }
}