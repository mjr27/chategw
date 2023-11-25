using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core.Problems;

using HotChocolate.Resolvers;
using HotChocolate.Types;
namespace Egw.PubManagement.Application.GraphQl.Types;

/// <inheritdoc />
public class PublicationChapterType : ObjectType<PublicationChapterDto>
{
    /// <inheritdoc />
    protected override void Configure(IObjectTypeDescriptor<PublicationChapterDto> descriptor)
    {
        descriptor
            .Field("paragraph")
            .Description("Chapter title paragraph")
            .Type<NonNullType<ParagraphType>>()
            .Resolve(HandleTitleParagraph);
    }

    private static async Task<ParagraphDto> HandleTitleParagraph(IResolverContext ctx, CancellationToken ct)
    {
        PublicationChapterDto parent = ctx.Parent<PublicationChapterDto>();
        return await ctx.DataLoader<ParagraphByIdLoader>().LoadAsync(parent.ParaId, ct)
               ?? throw new NotFoundProblemDetailsException("Main chapter paragraph not found");
    }
}