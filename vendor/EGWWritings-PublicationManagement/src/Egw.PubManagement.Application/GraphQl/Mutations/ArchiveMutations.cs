using Egw.PubManagement.Application.Messaging.Archive;
using Egw.PubManagement.Core;

using HotChocolate;
using HotChocolate.Types;

using MediatR;

namespace Egw.PubManagement.Application.GraphQl.Mutations;

/// <summary>
/// Archive-related mutations
/// </summary>
[ExtendObjectType(typeof(GraphQlMutations))]
public class ArchiveMutations
{
    /// <summary>
    /// Sets publication permissions
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> SavePublicationToArchive(
        [Argument] SavePublicationToArchiveInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }

    /// <summary>
    /// Delete archive entry
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteEntryFromArchive(
        [Argument] DeleteEntryFromArchiveInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }

    /// <summary>
    /// Restores archive entry
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> RestoreEntryFromArchive(
        [Argument] RestoreEntryFromArchiveInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }
}