using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence.Entities;

namespace Egw.PubManagement.Application.Services.Filters;

/// <summary>
/// Default projector
/// </summary>
public class DefaultFilter :
    IEntityPrefilter<ArchivedPublication>,
    IEntityPrefilter<Cover>,
    IEntityPrefilter<CoverType>,
    IEntityPrefilter<Language>,
    IEntityPrefilter<ExportedFile>,
    IEntityPrefilter<Folder>,
    IEntityPrefilter<FolderType>,
    IEntityPrefilter<Paragraph>,
    IEntityPrefilter<Publication>,
    IEntityPrefilter<PublicationAuthor>,
    IEntityPrefilter<PublicationChapter>,
    IEntityPrefilter<Mp3File>,
    IEntityPrefilter<PublicationSeries>
{
    /// <inheritdoc />
    public Task<IQueryable<ArchivedPublication>> Filter(IQueryable<ArchivedPublication> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult<IQueryable<ArchivedPublication>>(queryable
            .Where(r => r.DeletedAt == null)
            .OrderBy(r => r.PublicationId)
            .ThenByDescending(r => r.ArchivedAt));

    /// <inheritdoc />
    public Task<IQueryable<Language>> Filter(IQueryable<Language> queryable, CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<ExportedFile>> Filter(IQueryable<ExportedFile> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<Folder>> Filter(IQueryable<Folder> queryable, CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<FolderType>> Filter(IQueryable<FolderType> queryable, CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<Paragraph>> Filter(IQueryable<Paragraph> queryable, CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<Publication>> Filter(IQueryable<Publication> queryable, CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<PublicationAuthor>> Filter(IQueryable<PublicationAuthor> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<PublicationChapter>> Filter(IQueryable<PublicationChapter> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<Mp3File>> Filter(IQueryable<Mp3File> queryable, CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<PublicationSeries>> Filter(IQueryable<PublicationSeries> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<Cover>> Filter(IQueryable<Cover> queryable, CancellationToken cancellationToken)
        => Task.FromResult(queryable);

    /// <inheritdoc />
    public Task<IQueryable<CoverType>> Filter(IQueryable<CoverType> queryable, CancellationToken cancellationToken)
        => Task.FromResult(queryable);
}