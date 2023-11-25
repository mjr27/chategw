using System.Diagnostics.CodeAnalysis;

using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence;

using GreenDonut;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.GraphQl.Loaders;

/// <inheritdoc />
public class FolderRecursivePublicationCountByIdLoader : BatchDataLoader<int, int>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;

    /// <inheritdoc />
    public FolderRecursivePublicationCountByIdLoader(
        IDbContextFactory<PublicationDbContext> dbContextFactory,
        IBatchScheduler batchScheduler, DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }


    /// <inheritdoc />
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    protected override async Task<IReadOnlyDictionary<int, int>> LoadBatchAsync(IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var results = db.Folders
            .Where(r => keys.Contains(r.Id))
            .Select(r => new
            {
                r.Id,
                Count = db.PublicationPlacement
                    .Count(
                        p => db.Folders
                            .Where(f => f.MaterializedPath.StartsWith(r.MaterializedPath))
                            .Select(f => f.Id)
                            .Contains(p.FolderId)
                    )
            });
        return await results.ToLoaderResult(keys, r => r.Id, r => r.Count, cancellationToken);
    }
}