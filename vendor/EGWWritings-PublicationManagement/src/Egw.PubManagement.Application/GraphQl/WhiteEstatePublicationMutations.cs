using Egw.PubManagement.Application.Messaging.Import;
using Egw.PubManagement.Core;

using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;

using MediatR;
namespace Egw.PubManagement.Application.GraphQl;

/// <summary>
/// Mutations
/// </summary>
[ExtendObjectType(typeof(GraphQlMutations))]
[Authorize]
public class WhiteEstatePublicationMutations
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="publicationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [UseMutationConvention]
    public async Task<bool> RecalculatePublication([Service] IMediator mediator,
        [Argument] int publicationId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new RecalculatePublicationMetadataCommand(publicationId), cancellationToken);
        await mediator.Send(new RecalculateTocPublicationCommand(publicationId), cancellationToken);
        return true;
    }

}