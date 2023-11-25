namespace Egw.PubManagement.Application.Messaging.Import;

/// <summary>
/// Recalculates links for a given publication
/// </summary>
/// <param name="PublicationId">Publication Id</param>
public record RecalculateLinksCommand(int PublicationId) : IApplicationCommand;