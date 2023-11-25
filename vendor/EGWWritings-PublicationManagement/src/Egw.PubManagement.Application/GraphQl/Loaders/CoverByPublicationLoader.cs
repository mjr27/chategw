using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using GreenDonut;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.GraphQl.Loaders;

/// <summary>
/// Loads <see cref="CoverTypeDto"/> by Id
/// </summary>
public class CoverByPublicationLoader : GroupedDataLoader<int, CoverDto>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IEntityProjector<Cover, CoverDto> _projector;
    private readonly IEntityPrefilter<Cover> _filter;

    /// <inheritdoc />
    public CoverByPublicationLoader(
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
    protected override async Task<ILookup<int, CoverDto>> LoadGroupedBatchAsync(IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        IQueryable<Cover> query = db.Covers.Where(r => keys.Contains(r.PublicationId) && r.IsMain);
        IQueryable<CoverDto> result = await query.ApplyPipeline(_filter, _projector, cancellationToken);
        List<CoverDto> publicationCovers = await result.ToListAsync(cancellationToken);
        return publicationCovers.ToLookup(r => r.PublicationId);
    }
}