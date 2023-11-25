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
public class CoverByIdLoader : BatchDataLoader<Guid, CoverDto?>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IEntityProjector<Cover, CoverDto> _projector;
    private readonly IEntityPrefilter<Cover> _filter;

    /// <inheritdoc />
    public CoverByIdLoader(
        IDbContextFactory<PublicationDbContext> dbContextFactory,
        IEntityProjector<Cover, CoverDto> projector,
        IEntityPrefilter<Cover> filter,
        IBatchScheduler batchScheduler, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _projector = projector;
        _filter = filter;
    }

    /// <inheritdoc />
    protected override async Task<IReadOnlyDictionary<Guid, CoverDto?>> LoadBatchAsync(IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        IQueryable<Cover> query = db.Covers.Where(r => keys.Contains(r.Id));
        IQueryable<CoverDto> result = await query.ApplyPipeline(_filter, _projector, cancellationToken);
        return await result.ToLoaderResult(keys, r => r.Id, cancellationToken);
    }
}