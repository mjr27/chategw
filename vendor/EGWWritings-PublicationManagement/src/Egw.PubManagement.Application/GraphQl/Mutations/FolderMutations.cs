using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Messaging.Folders;
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
public class FolderMutations
{

    /// <summary>
    /// Recalculates global order for folders 
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> RecalculateFolders([Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new RecalculateFoldersInput(), cancellationToken);
        return true;
    }

    /// <summary>
    /// Creates a new folder at the end of specified parent folder
    /// </summary>
    public async Task<bool> CreateFolder(
        [Argument] CreateFolderInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }

    /// <summary>
    /// Delete a folder by ID
    /// </summary>
    public async Task<bool> DeleteFolder(
        [Argument] DeleteFolderInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }

    /// <summary>
    /// Rename a folder
    /// </summary>
    public async Task<FolderDto?> RenameFolder(
        [Argument] RenameFolderInput input,
        [Service] IMediator mediator,
        FolderByIdLoader loader,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.Id, cancellationToken);
    }

    /// <summary>
    /// Changes folder type
    /// </summary>
    public async Task<FolderDto?> ChangeFolderType(
        [Argument] ChangeFolderTypeInput input,
        [Service] IMediator mediator,
        FolderByIdLoader loader,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.Id, cancellationToken);
    }

    /// <summary>
    /// Moves a folder to a different parent or changes order
    /// </summary>
    public async Task<FolderDto?> MoveFolder(
        [Argument] MoveFolderInput input,
        [Service] IMediator mediator,
        FolderByIdLoader loader,
        CancellationToken cancellationToken)
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.Id, cancellationToken);
    }
}