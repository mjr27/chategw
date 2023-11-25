using Egw.PubManagement.Application.Messaging.Files;
using Egw.PubManagement.Core;

using HotChocolate;
using HotChocolate.Types;

using MediatR;

namespace Egw.PubManagement.Application.GraphQl.Mutations;

/// <summary>
/// Archive-related mutations
/// </summary>
[ExtendObjectType(typeof(GraphQlMutations))]
public class ExportMutations
{
    /// <summary>
    /// Upload a file as an export
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> UploadExport(
        [Argument] UploadExportInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }

    /// <summary>
    /// Export
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ExportEpub(
        [Argument] ExportEpubInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }

    /// <summary>
    /// Sets main export file
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> SetMainExportFile(
        [Argument] SetMainExportFileInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }

    /// <summary>
    /// Sets main export file
    /// </summary>
    /// <param name="input">Input object</param>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteExportFile(
        [Argument] DeleteExportFileInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return true;
    }
}