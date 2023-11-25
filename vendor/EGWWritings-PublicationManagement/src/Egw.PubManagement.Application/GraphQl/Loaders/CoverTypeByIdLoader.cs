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
public class CoverTypeByIdLoader : BatchDataLoader<string, CoverTypeDto?>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IEntityProjector<CoverType, CoverTypeDto> _projector;
    private readonly IEntityPrefilter<CoverType> _filter;

    /// <inheritdoc />
    public CoverTypeByIdLoader(
        IDbContextFactory<PublicationDbContext> dbContextFactory,
        IEntityProjector<CoverType, CoverTypeDto> projector,
        IEntityPrefilter<CoverType> filter,
        IBatchScheduler batchScheduler, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _projector = projector;
        _filter = filter;
    }

    /// <inheritdoc />
    protected override async Task<IReadOnlyDictionary<string, CoverTypeDto?>> LoadBatchAsync(IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        IQueryable<CoverType> query = db.CoverTypes.Where(r => keys.Contains(r.Code));
        IQueryable<CoverTypeDto> result = await query.ApplyPipeline(_filter, _projector, cancellationToken);
        return await result.ToLoaderResult(keys, r => r.Code, cancellationToken);
    }
}