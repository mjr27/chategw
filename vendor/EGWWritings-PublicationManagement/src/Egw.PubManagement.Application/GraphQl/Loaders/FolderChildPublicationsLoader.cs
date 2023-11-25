using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using GreenDonut;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.GraphQl.Loaders;

/// <inheritdoc />
public class FolderChildPublicationsLoader : GroupedDataLoader<int, PublicationDto>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IEntityProjector<Publication, PublicationDto> _projector;
    private readonly IEntityPrefilter<Publication> _filter;

    /// <inheritdoc />
    public FolderChildPublicationsLoader(
        IDbContextFactory<PublicationDbContext> dbContextFactory,
        IEntityProjector<Publication, PublicationDto> projector,
        IEntityPrefilter<Publication> filter,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _projector = projector;
        _filter = filter;
    }

    /// <inheritdoc />
    protected override async Task<ILookup<int, PublicationDto>> LoadGroupedBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory
            .CreateDbContextAsync(cancellationToken);
        IQueryable<Publication> query = db.Publications
            .Include(r => r.Placement)
            .Where(r => keys.Contains(r.Placement!.FolderId))
            .OrderBy(r => r.Placement!.Order);
        IQueryable<PublicationDto> result = await query
            .ApplyPipeline(_filter, _projector, cancellationToken);
        List<PublicationDto> foundPublications = await result
            .ToListAsync(cancellationToken);
        return foundPublications.ToLookup(r => r.FolderId!.Value);
    }
}