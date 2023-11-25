using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using GreenDonut;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.GraphQl.Loaders;

/// <summary>
/// Loads <see cref="FolderDto"/> by Id
/// </summary>
public class AuthorByIdLoader : BatchDataLoader<int, PublicationAuthorDto?>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IEntityProjector<PublicationAuthor, PublicationAuthorDto> _projector;
    private readonly IEntityPrefilter<PublicationAuthor> _filter;

    /// <inheritdoc />
    public AuthorByIdLoader(
        IDbContextFactory<PublicationDbContext> dbContextFactory,
        IEntityProjector<PublicationAuthor, PublicationAuthorDto> projector,
        IEntityPrefilter<PublicationAuthor> filter,
        IBatchScheduler batchScheduler, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _projector = projector;
        _filter = filter;
    }

    /// <inheritdoc />
    protected override async Task<IReadOnlyDictionary<int, PublicationAuthorDto?>> LoadBatchAsync(IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        IQueryable<PublicationAuthor> query = db.Authors.Where(r => keys.Contains(r.Id));
        IQueryable<PublicationAuthorDto> result = await query.ApplyPipeline(_filter, _projector, cancellationToken);
        return await result.ToLoaderResult(keys, r => r.Id, cancellationToken);
    }
}