using Egw.PubManagement.Persistence.Enums;
namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary>
/// Sets permission for a publication
/// </summary>
public class SetPublicationPermissionInput : IApplicationCommand
{
    /// <summary>Publication id</summary>
    public required int PublicationId { get; init; }
    /// <summary>New permission</summary>
    public required PublicationPermissionEnum Permission { get; init; }
}