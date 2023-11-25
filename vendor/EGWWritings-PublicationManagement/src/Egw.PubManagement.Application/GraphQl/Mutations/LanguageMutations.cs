using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Messaging.Languages;
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
public class LanguageMutations
{
    /// <summary>
    /// Creates a new language
    /// </summary>
    /// <param name="input">Mutation input</param>
    /// <param name="mediator"></param>
    /// <param name="loader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<LanguageDto?> CreateLanguage(
        [Argument] CreateLanguageInput input,
        [Service] IMediator mediator,
        LanguageByCodeLoader loader,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.Code, cancellationToken);
    }

    /// <summary>
    /// Changes language details
    /// </summary>
    /// <param name="input">Mutation input</param>
    /// <param name="mediator"></param>
    /// <param name="loader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<LanguageDto?> UpdateLanguage(
        [Argument] UpdateLanguageInput input,
        [Service] IMediator mediator,
        LanguageByCodeLoader loader,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.Code, cancellationToken);
    }

    /// <summary>
    /// Changes language details
    /// </summary>
    /// <param name="input">Mutation input</param>
    /// <param name="mediator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<LanguageDto?> DeleteLanguage(
        [Argument] DeleteLanguageInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return null;
    }

    /// <summary>
    /// Create a new root folder for a language
    /// </summary>
    /// <param name="input">Mutation input</param>
    /// <param name="mediator"></param>
    /// <param name="loader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<LanguageDto?> CreateRootFolderForLanguage(
        [Argument] CreateRootFolderForLanguageInput input,
        [Service] IMediator mediator,
        LanguageByCodeLoader loader,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(input, cancellationToken);
        return await loader.LoadAsync(input.Code, cancellationToken);
    }
}