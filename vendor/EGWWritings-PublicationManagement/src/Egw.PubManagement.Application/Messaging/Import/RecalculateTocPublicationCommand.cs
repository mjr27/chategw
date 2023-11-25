using Egw.PubManagement.Core.Messaging;
using Egw.PubManagement.Persistence;

using MediatR;
namespace Egw.PubManagement.Application.Messaging.Import;

/// <summary>
/// Recalculates table of contents for a publication
/// </summary>
/// <param name="PublicationId">Publication ID</param>
public record RecalculateTocPublicationCommand(int PublicationId) : IRequest,
    ITransactionalCommand<PublicationDbContext>;