using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using GreenDonut;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.GraphQl.Loaders;

/// <summary>
/// Loads <see cref="FolderTypeDto"/> by Id
/// </summary>
public class FolderTypeByIdLoader : BatchDataLoader<string, FolderTypeDto?>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IEntityProjector<FolderType, FolderTypeDto> _projector;
    private readonly IEntityPrefilter<FolderType> _filter;

    /// <inheritdoc />
    public FolderTypeByIdLoader(
        IDbContextFactory<PublicationDbContext> dbContextFactory,
        IEntityProjector<FolderType, FolderTypeDto> projector,
        IEntityPrefilter<FolderType> filter,
        IBatchScheduler batchScheduler, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _projector = projector;
        _filter = filter;
    }

    /// <inheritdoc />
    protected override async Task<IReadOnlyDictionary<string, FolderTypeDto?>> LoadBatchAsync(IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        IQueryable<FolderType> query = db.FolderTypes.Where(r => keys.Contains(r.FolderTypeId));
        IQueryable<FolderTypeDto> result = await query.ApplyPipeline(_filter, _projector, cancellationToken);
        return await result.ToLoaderResult(keys, r => r.Id, cancellationToken);
    }
}