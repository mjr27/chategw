using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.GraphQl.Types;

/// <inheritdoc />
public class PublicationType : ObjectType<PublicationDto>
{
    /// <inheritdoc />
    protected override void Configure(IObjectTypeDescriptor<PublicationDto> descriptor)
    {
        descriptor
            .Field("author")
            .Description("Publication author")
            .Resolve(HandlePublicationAuthor);
        descriptor
            .Field("language")
            .Description("Publication language")
            .Type<NonNullType<LanguageType>>()
            .Resolve(HandleLanguage);
        descriptor
            .Field("chapters")
            .Type<ListType<NonNullType<PublicationChapterType>>>()
            .Description("Publication chapters")
            .UseDbContext<PublicationDbContext>()
            .UseProjection()
            .Resolve(HandleChapters);
        descriptor
            .Field("mp3Files")
            .Description("Publication mp3 files")
            .Type<ListType<NonNullType<PublicationMp3FileType>>>()
            .UseDbContext<PublicationDbContext>()
            .UseProjection()
            .Resolve(HandleMp3Files);
        descriptor
            .Field("originalPublication")
            .Description("Original publication")
            .Resolve(HandleOriginalPublication);
        descriptor
            .Field("paragraphs")
            .Type<ListType<NonNullType<ParagraphType>>>()
            .Argument("start", a => a.Type<NonNullType<IntType>>().Description("Start element"))
            .Argument("skip", a => a.Type<IntType>().Description("Offset compared to start element"))
            .Argument("take", a => a.Type<IntType>().Description("How many elements to take"))
            .UseDbContext<PublicationDbContext>()
            .UseProjection()
            .Description("Paragraphs")
            .Resolve(HandleListParagraphs);
        descriptor
            .Field("covers")
            .Description("Publication covers")
            .Resolve(HandlePublicationCovers);
        descriptor
            .Field("webCover")
            .Description("Web publication cover")
            .Resolve(HandlePublicationCover);
        descriptor
            .Field("topDepth")
            .Description("TOC depth")
            .Type<IntType>()
            .Resolve(HandleTocDepth);
        descriptor
            .Field("exports")
            .Description("Exported files")
            .UseDbContext<PublicationDbContext>()
            .Resolve(HandleExports);
        descriptor
            .Field("archive")
            .Description("Archived versions")
            .UseDbContext<PublicationDbContext>()
            .UseOffsetPaging<ArchivedPublicationType>(options: new PagingOptions { IncludeTotalCount = true })
            .UseProjection()
            .UseFiltering()
            .Resolve((ctx, ct) =>
            {
                PublicationDbContext db = ctx.DbContext<PublicationDbContext>();
                IEntityProjector<ArchivedPublication, ArchivedPublicationDto> projector =
                    ctx.Service<IEntityProjector<ArchivedPublication, ArchivedPublicationDto>>();
                IEntityPrefilter<ArchivedPublication> filter = ctx.Service<IEntityPrefilter<ArchivedPublication>>();
                int publicationId = ctx.Parent<PublicationDto>().PublicationId;
                IQueryable<ArchivedPublication> children =
                    db.PublicationArchive.Where(r => r.PublicationId == publicationId)
                        .OrderByDescending(r => r.ArchivedAt);

                children = filter.Filter(children, ct).Result;
                return projector.Project(children, ct).Result;
            });
    }

    private static async Task<PublicationDto?> HandleOriginalPublication(IResolverContext ctx, CancellationToken ct)
    {
        PublicationDto parent = ctx.Parent<PublicationDto>();
        if (parent.OriginalPublicationId is null)
        {
            return null;
        }

        return await ctx.DataLoader<PublicationByIdLoader>().LoadAsync(parent.OriginalPublicationId.Value, ct);
    }

    private static async Task<LanguageDto?> HandleLanguage(IResolverContext ctx, CancellationToken ct)
    {
        PublicationDto parent = ctx.Parent<PublicationDto>();
        return await ctx.DataLoader<LanguageByCodeLoader>().LoadAsync(parent.LanguageCode, ct);
    }

    private static async Task<PublicationAuthorDto?> HandlePublicationAuthor(IResolverContext ctx, CancellationToken ct)
    {
        PublicationDto parent = ctx.Parent<PublicationDto>();
        if (parent.AuthorId is null)
        {
            return null;
        }

        return await ctx.DataLoader<AuthorByIdLoader>().LoadAsync(parent.AuthorId.Value, ct);
    }

    private static async Task<IQueryable<PublicationChapterDto>> HandleChapters(
        IResolverContext ctx,
        CancellationToken ct)
    {
        int publicationId = ctx.Parent<PublicationDto>().PublicationId;
        PublicationDbContext db = ctx.DbContext<PublicationDbContext>();
        IEntityPrefilter<PublicationChapter> filter = ctx.Service<IEntityPrefilter<PublicationChapter>>();
        IEntityProjector<PublicationChapter, PublicationChapterDto> projector =
            ctx.Service<IEntityProjector<PublicationChapter, PublicationChapterDto>>();
        IQueryable<PublicationChapter> chaptersRaw = await filter.Filter(db.PublicationChapters
            .Where(r => r.PublicationId == publicationId)
            .OrderBy(r => r.Order), ct);
        return await projector.Project(chaptersRaw, ct);
    }

    private static async Task<IQueryable<PublicationMp3FileDto>> HandleMp3Files(
        IResolverContext ctx,
        CancellationToken ct)
    {
        int publicationId = ctx.Parent<PublicationDto>().PublicationId;
        PublicationDbContext db = ctx.DbContext<PublicationDbContext>();
        IEntityPrefilter<Mp3File> filter = ctx.Service<IEntityPrefilter<Mp3File>>();
        IEntityProjector<Mp3File, PublicationMp3FileDto> projector =
            ctx.Service<IEntityProjector<Mp3File, PublicationMp3FileDto>>();
        IQueryable<Mp3File> chaptersRaw = await filter.Filter(db.PublicationMp3Files
            .Where(r => r.PublicationId == publicationId)
            .OrderBy(r => r.Filename), ct);
        return await projector.Project(chaptersRaw, ct);
    }

    private static async Task<IQueryable<ParagraphDto>?> HandleListParagraphs(
        IResolverContext ctx,
        CancellationToken ct)
    {
        PublicationDbContext db = ctx.DbContext<PublicationDbContext>();
        IEntityPrefilter<Paragraph> filter = ctx.Service<IEntityPrefilter<Paragraph>>();
        IEntityProjector<Paragraph, ParagraphDto> projector =
            ctx.Service<IEntityProjector<Paragraph, ParagraphDto>>();
        int publicationId = ctx.Parent<PublicationDto>().PublicationId;
        int elementId = ctx.ArgumentValue<int>("start");
        int? offset = ctx.ArgumentValue<int?>("skip");
        int? take = ctx.ArgumentValue<int?>("take");
        var queries = new WhiteEstatePublicationQueries();

        return await queries.GetOrderedParagraphs(
            new ParaId(publicationId, elementId),
            offset,
            take,
            projector,
            filter,
            db,
            ct
        );
    }

    private static async Task<CoverDto[]> HandlePublicationCovers(
        IResolverContext ctx,
        CancellationToken ct)
    {
        int parentId = ctx.Parent<PublicationDto>().PublicationId;
        CoverByPublicationLoader loader = ctx.DataLoader<CoverByPublicationLoader>();
        return await loader.LoadAsync(parentId, ct);
    }

    private static async Task<CoverDto?> HandlePublicationCover(
        IResolverContext ctx,
        CancellationToken ct)
    {
        int parentId = ctx.Parent<PublicationDto>().PublicationId;
        CoverByPublicationLoader loader = ctx.DataLoader<CoverByPublicationLoader>();
        CoverDto[] publicationCovers = await loader.LoadAsync(parentId, ct);
        return publicationCovers.FirstOrDefault(r => r is { TypeId: "web", IsMain: true });
    }

    private static async Task<int?> HandleTocDepth(
        IResolverContext ctx,
        CancellationToken ct)
    {
        PublicationDbContext db = ctx.DbContext<PublicationDbContext>();
        int publicationId = ctx.Parent<PublicationDto>().PublicationId;
        return await db.Publications
            .Where(r => r.PublicationId == publicationId)
            .Select(r => r.Placement == null ? null : r.Placement.TocDepth)
            .FirstOrDefaultAsync(ct);
    }

    private static async Task<List<ExportedFileDto>> HandleExports(
        IResolverContext ctx,
        CancellationToken ct)
    {
        PublicationDbContext db = ctx.DbContext<PublicationDbContext>();
        int publicationId = ctx.Parent<PublicationDto>().PublicationId;
        IQueryable<ExportedFile> query = db.PublicationExports
            .Where(r => r.PublicationId == publicationId && r.IsMain);
        query = await ctx.Service<IEntityPrefilter<ExportedFile>>().Filter(query, ct);
        IQueryable<ExportedFileDto> projected =
            await ctx.Service<IEntityProjector<ExportedFile, ExportedFileDto>>().Project(query, ct);
        return await projected.ToListAsync(ct);
    }
}