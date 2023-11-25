using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Messaging.Publications;
using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;

using HotChocolate;
using HotChocolate.Types;

using MediatR;

namespace Egw.PubManagement.Application.GraphQl.Mutations;

/// <summary>
/// Publication-related mutations
/// </summary>
[ExtendObjectType(typeof(GraphQlMutations))]
public class PublicationMutations
{
    /// <summary>
    /// Sets publication permissions
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="loader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PublicationDto?> SetPublicationPermission(
        [Argument] SetPublicationPermissionInput input,
        [Service] IMediator mediator,
        PublicationByIdLoader loader,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.PublicationId, cancellationToken);
    }

    /// <summary>
    /// Sets publication permissions
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="loader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PublicationDto?> SetPublicationOrder(
        [Argument] SetPublicationOrderInput input,
        [Service] IMediator mediator,
        PublicationByIdLoader loader,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.PublicationId, cancellationToken);
    }

    /// <summary>
    /// Publishes a publication
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="loader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PublicationDto?> PublishPublication(
        [Argument] PublishPublicationInput input,
        [Service] IMediator mediator,
        PublicationByIdLoader loader,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.PublicationId, cancellationToken);
    }

    /// <summary>
    /// Unpublishes a publication
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="loader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PublicationDto?> UnpublishPublication(
        [Argument] UnpublishPublicationInput input,
        [Service] IMediator mediator,
        PublicationByIdLoader loader,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.PublicationId, cancellationToken);
    }

    /// <summary>
    /// Update following list of publication details: 
    /// code
    /// title(plain text)
    /// publisher
    /// publication year
    /// author
    /// description(in weml format)
    /// page count
    /// ISBN code
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="loader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PublicationDto?> SetPublicationMetadata(
        [Argument] SetPublicationMetadataInput input,
        [Service] IMediator mediator,
        PublicationByIdLoader loader,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.PublicationId, cancellationToken);
    }

    /// <summary>
    /// Set max toc depth for a publication
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="loader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PublicationDto?> SetPublicationTocDepth(
        [Argument] SetPublicationTocDepthInput input,
        [Service] IMediator mediator,
        PublicationByIdLoader loader,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.PublicationId, cancellationToken);
    }

    /// <summary>
    /// Move publication to a different folder
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="loader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PublicationDto?> MovePublication(
        [Argument] MovePublicationInput input,
        [Service] IMediator mediator,
        PublicationByIdLoader loader,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.PublicationId, cancellationToken);
    }
}