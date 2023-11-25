using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Messaging.Import;
using Egw.PubManagement.Application.Messaging.Paragraphs;
using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;

using HotChocolate;
using HotChocolate.Types;

using MediatR;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.GraphQl.Mutations
{
    /// <summary>
    /// Language-related mutations
    /// </summary>
    [ExtendObjectType(typeof(GraphQlMutations))]
    public class ParagraphMutations
    {
        /// <summary>
        /// Delete paragraph
        /// </summary>
        /// <param name="input">Mutation input</param>
        /// <param name="mediator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ParaId> DeleteParagraph(
            [Argument] DeleteParagraphInput input,
            [Service] IMediator mediator,
            CancellationToken cancellationToken
        )
        {
            await mediator.Send(input, cancellationToken);
            await mediator.Send(new RecalculatePublicationMetadataCommand(input.ParaId.PublicationId), cancellationToken);
            await mediator.Send(new RecalculateTocPublicationCommand(input.ParaId.PublicationId), cancellationToken);
            return input.ParaId;
        }

        /// <summary>
        /// Update paragraph content 
        /// </summary>
        /// <param name="input">Mutation input</param>
        /// <param name="mediator"></param>
        /// <param name="loader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ParagraphDto?> SetParagraphContent(
            [Argument] SetParagraphContentInput input,
            [Service] IMediator mediator,
            ParagraphByIdLoader loader,
            CancellationToken cancellationToken
        )
        {
            await mediator.Send(input, cancellationToken);
            await mediator.Send(new RecalculatePublicationMetadataCommand(input.ParaId.PublicationId), cancellationToken);
            await mediator.Send(new RecalculateTocPublicationCommand(input.ParaId.PublicationId), cancellationToken);
            return await loader.LoadAsync(input.ParaId, cancellationToken); 
        }

        /// <summary>
        /// Add paragraph
        /// </summary>
        /// <param name="input">Mutation input</param>
        /// <param name="mediator"></param>
        /// <param name="loader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ParagraphDto?> AddParagraph(
            [Argument] AddParagraphInput input,
            [Service] IMediator mediator,
            ParagraphByPublicationLoader loader,
            CancellationToken cancellationToken
        )
        {
            await mediator.Send(input, cancellationToken);
            await mediator.Send(new RecalculatePublicationMetadataCommand(input.PublicationId), cancellationToken);
            await mediator.Send(new RecalculateTocPublicationCommand(input.PublicationId), cancellationToken);
            return await loader.LoadAsync(input.PublicationId, cancellationToken);
        }
    }
}
