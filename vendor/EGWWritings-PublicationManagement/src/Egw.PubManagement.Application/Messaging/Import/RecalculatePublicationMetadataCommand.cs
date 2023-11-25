using Egw.PubManagement.Core.Messaging;
using Egw.PubManagement.Persistence;

using MediatR;
namespace Egw.PubManagement.Application.Messaging.Import;

/// <summary>
/// Recalculates publication metadata
/// </summary>
/// <param name="PublicationId">Publication ID</param>
public record RecalculatePublicationMetadataCommand(int PublicationId) : IRequest,
    ITransactionalCommand<PublicationDbContext>;