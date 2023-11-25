using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core;
using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.GraphQl;

/// <summary>
/// 
/// </summary>
[ExtendObjectType(typeof(GraphQlQueries))]
public class WhiteEstatePublicationQueries
{
    /// <summary>
    /// Retrieves list of languages
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UseOptionalPagination]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<PublicationAuthorDto>> GetAuthors(
        [Service] IEntityProjector<PublicationAuthor, PublicationAuthorDto> projector,
        [Service] IEntityPrefilter<PublicationAuthor> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.Authors, filter, projector, cancellationToken);


    /// <summary>
    /// Retrieves list of exported files
    /// </summary>
    /// <param name="projector">Projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UsePagination]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<ExportedFileDto>> GetExportedFiles(
        [Service] IEntityProjector<ExportedFile, ExportedFileDto> projector,
        [Service] IEntityPrefilter<ExportedFile> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.PublicationExports, filter, projector, cancellationToken);

    /// <summary>
    /// Retrieves list of languages
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<LanguageDto>> GetLanguages(
        [Service] IEntityProjector<Language, LanguageDto> projector,
        [Service] IEntityPrefilter<Language> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.Languages, filter, projector, cancellationToken);


    /// <summary>
    /// Retrieves list of folders
    /// </summary>
    /// <param name="folderId">Folder Id</param>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UseFirstOrDefault]
    [UseProjection]
    public Task<IQueryable<FolderDto>> GetFolder(
        [Argument] int folderId,
        [Service] IEntityProjector<Folder, FolderDto> projector,
        [Service] IEntityPrefilter<Folder> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.Folders.Where(r => r.Id == folderId), filter, projector, cancellationToken);


    /// <summary>
    /// Retrieves list of folders
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<FolderDto>> GetFolders(
        [Service] IEntityProjector<Folder, FolderDto> projector,
        [Service] IEntityPrefilter<Folder> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.Folders, filter, projector, cancellationToken);

    /// <summary>
    /// Retrieves list of languages
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<FolderTypeDto>> GetFolderTypes(
        [Service] IEntityProjector<FolderType, FolderTypeDto> projector,
        [Service] IEntityPrefilter<FolderType> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.FolderTypes, filter, projector, cancellationToken);

    /// <summary>
    /// Retrieves list of paragraphs
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UsePagination]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<ParagraphDto>> GetParagraphs(
        [Service] IEntityProjector<Paragraph, ParagraphDto> projector,
        [Service] IEntityPrefilter<Paragraph> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.Paragraphs, filter, projector, cancellationToken);


    /// <summary>
    /// Retrieves list of paragraphs
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="paraId">Start paragraph</param>
    /// <param name="offset">Offset relative to start paragraph</param>
    /// <param name="take">Number of paragraphs to take</param>
    [Authorize]
    [UseDbContext(typeof(PublicationDbContext))]
    [UseProjection]
    public async Task<IQueryable<ParagraphDto>?> GetOrderedParagraphs(
        [Argument] ParaId paraId,
        [Argument] int? offset,
        [Argument] int? take,
        [Service] IEntityProjector<Paragraph, ParagraphDto> projector,
        [Service] IEntityPrefilter<Paragraph> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    )
    {
        offset ??= 0;
        take ??= 10;
        // int publicationId = ctx.Parent<PublicationDto>().PublicationId;
        // int elementId = ctx.ArgumentValue<int>("start");
        var orderRecord = await db.Paragraphs.Where(r => r.ParaId == paraId)
            .Select(r => new { r.Order })
            .FirstOrDefaultAsync(cancellationToken);
        if (orderRecord is null)
        {
            return null;
        }

        int targetOrder = orderRecord.Order + offset.Value;
        IQueryable<Paragraph> paragraphs = db.Paragraphs
            .Where(r => r.PublicationId == paraId.PublicationId && r.Order >= targetOrder)
            .OrderBy(r => r.Order)
            .Take(take.Value);
        return await ApplyPipeline(paragraphs, filter, projector, cancellationToken);
    }

    /// <summary>
    /// Retrieves list of publications
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UsePagination]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<PublicationDto>> GetPublications(
        [Service] IEntityProjector<Publication, PublicationDto> projector,
        [Service] IEntityPrefilter<Publication> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.Publications, filter, projector, cancellationToken);

    /// <summary>
    /// Retrieves list of publications
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UsePagination]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<ArchivedPublicationDto>> GetPublicationArchive(
        [Service] IEntityProjector<ArchivedPublication, ArchivedPublicationDto> projector,
        [Service] IEntityPrefilter<ArchivedPublication> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.PublicationArchive, filter, projector, cancellationToken);

    /// <summary>
    /// Retrieves list of publications
    /// </summary>
    /// <param name="publicationId">Publication Id</param>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UseFirstOrDefault]
    [UseProjection]
    public Task<IQueryable<PublicationDto>> GetPublication(
        [Argument] int publicationId,
        [Service] IEntityProjector<Publication, PublicationDto> projector,
        [Service] IEntityPrefilter<Publication> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(
        db.Publications.Where(r => r.PublicationId == publicationId),
        filter, projector, cancellationToken);

    /// <summary>
    /// Retrieves list of publication chapters
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UsePagination]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<PublicationChapterDto>> GetPublicationChapters(
        [Service] IEntityProjector<PublicationChapter, PublicationChapterDto> projector,
        [Service] IEntityPrefilter<PublicationChapter> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.PublicationChapters.OrderBy(r => r.PublicationId).ThenBy(r => r.Order), filter, projector,
        cancellationToken);

    /// <summary>
    /// Retrieves list of publication mp3 files
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UsePagination]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<PublicationMp3FileDto>> GetPublicationMp3Files(
        [Service] IEntityProjector<Mp3File, PublicationMp3FileDto> projector,
        [Service] IEntityPrefilter<Mp3File> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.PublicationMp3Files, filter, projector, cancellationToken);

    /// <summary>
    /// Retrieves list of folders
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UseOptionalPagination]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<PublicationSeriesDto>> GetPublicationSeries(
        [Service] IEntityProjector<PublicationSeries, PublicationSeriesDto> projector,
        [Service] IEntityPrefilter<PublicationSeries> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.PublicationSeries, filter, projector, cancellationToken);

    /// <summary>
    /// Retrieves list of covers
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UseOptionalPagination]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<CoverDto>> GetCovers(
        [Service] IEntityProjector<Cover, CoverDto> projector,
        [Service] IEntityPrefilter<Cover> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    )
    {
        IQueryable<CoverDto> data = await  ApplyPipeline(db.Covers, filter, projector, cancellationToken);
        return data.OrderBy(r => r.PublicationId);
    }

    /// <summary>
    /// Retrieves list of cover types
    /// </summary>
    /// <param name="projector">Entity projector</param>
    /// <param name="filter">Filter</param>
    /// <param name="db">Database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [UseOptionalPagination]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public Task<IQueryable<CoverTypeDto>> GetCoverTypes(
        [Service] IEntityProjector<CoverType, CoverTypeDto> projector,
        [Service] IEntityPrefilter<CoverType> filter,
        PublicationDbContext db,
        CancellationToken cancellationToken
    ) => ApplyPipeline(db.CoverTypes, filter, projector, cancellationToken);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="storageWrapper"></param>
    /// <param name="db"></param>
    /// <param name="publicationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    public async Task<List<Mp3FileItem>> GetMp3Files(
        [Service] IStorageWrapper storageWrapper,
        PublicationDbContext db, 
        int publicationId, CancellationToken cancellationToken)
    {
        return await new CloudMp3FilesLoader(storageWrapper).Get(publicationId, cancellationToken);
    }

    private static async Task<IQueryable<TOutput>> ApplyPipeline<TInput, TOutput>(
        IQueryable<TInput> queryable,
        IEntityPrefilter<TInput> filter,
        IEntityProjector<TInput, TOutput> projector,
        CancellationToken cancellationToken
    ) where TOutput : IProjectedDto<TInput>
    {
        IQueryable<TInput> filterQuery = await filter.Filter(queryable, cancellationToken);
        return await projector.Project(filterQuery, cancellationToken);
    }
}