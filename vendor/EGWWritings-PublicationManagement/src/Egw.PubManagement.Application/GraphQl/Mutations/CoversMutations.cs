using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Messaging.Covers;
using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;

using HotChocolate;
using HotChocolate.Types;

using MediatR;
namespace Egw.PubManagement.Application.GraphQl.Mutations;

/// <summary>
/// Language-related mutations
/// </summary>
[ExtendObjectType(typeof(GraphQlMutations))]
public class CoversMutations
{

    /// <summary>
    /// Recalculates global order for folders 
    /// </summary>
    public async Task<CoverDto?> UploadCover([Service] IMediator mediator,
        CoverByIdLoader loader,
        [Argument] UploadCoverInput input,
        CancellationToken cancellationToken)
    {
        input.Id ??= Guid.NewGuid();
        await mediator.Send(input, cancellationToken);
        return input.Id is null ? null : await loader.LoadAsync(input.Id.Value, cancellationToken);
    }

    /// <summary>
    /// Sets main cover of a specified type for a publication 
    /// </summary>
    /// <returns></returns>
    public async Task<CoverDto?> SetMainCover([Service] IMediator mediator,
        CoverByIdLoader loader,
        [Argument] SetMainCoverInput input,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return input.Id is null ? null : await loader.LoadAsync(input.Id.Value, cancellationToken);
    }

    /// <summary>
    /// Create cover type 
    /// </summary>
    /// <returns></returns>
    public async Task<CoverTypeDto?> CreateCoverType([Service] IMediator mediator,
        CoverTypeByIdLoader loader,
        [Argument] CreateCoverTypeInput input,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.Code, cancellationToken);
    }

    /// <summary>
    /// Update cover type 
    /// </summary>
    /// <returns></returns>
    public async Task<CoverTypeDto?> UpdateCoverType([Service] IMediator mediator,
        CoverTypeByIdLoader loader,
        [Argument] UpdateCoverTypeInput input,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.Code, cancellationToken);
    }

    /// <summary>
    /// Delete cover type 
    /// </summary>
    /// <returns></returns>
    public async Task<CoverTypeDto?> DeleteCoverType([Service] IMediator mediator,
        CoverTypeByIdLoader loader,
        [Argument] DeleteCoverTypeInput input,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.Code, cancellationToken);
    }
}