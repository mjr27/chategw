using Egw.PubManagement.Application.Messaging.Mp3;
using Egw.PubManagement.Core;

using HotChocolate;
using HotChocolate.Types;

using MediatR;

namespace Egw.PubManagement.Application.GraphQl.Mutations;

/// <summary>
/// Mp3 file related mutations
/// </summary>
[ExtendObjectType(typeof(GraphQlMutations))]
public class Mp3FileMutations
{
    /// <summary>
    /// Assign mp3 files to chapters
    /// </summary>
    /// <param name="input"></param>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> UpdateMp3Files(
        [Argument] UpdateMp3FilesInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }

    /// <summary>
    /// Create or Update manifest with mp3 files metadata
    /// </summary>
    /// <param name="input"></param>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> UpdateMp3Manifest([Argument] UpdateMp3ManifestInput input, 
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }
}