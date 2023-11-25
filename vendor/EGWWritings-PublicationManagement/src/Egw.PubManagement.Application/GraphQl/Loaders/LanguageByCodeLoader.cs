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
public class LanguageByCodeLoader : BatchDataLoader<string, LanguageDto?>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IEntityProjector<Language, LanguageDto> _projector;
    private readonly IEntityPrefilter<Language> _filter;

    /// <inheritdoc />
    public LanguageByCodeLoader(
        IDbContextFactory<PublicationDbContext> dbContextFactory,
        IEntityProjector<Language, LanguageDto> projector,
        IEntityPrefilter<Language> filter,
        IBatchScheduler batchScheduler, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _projector = projector;
        _filter = filter;
    }

    /// <inheritdoc />
    protected override async Task<IReadOnlyDictionary<string, LanguageDto?>> LoadBatchAsync(IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        IQueryable<Language> query = db.Languages.Where(r => keys.Contains(r.Code));
        IQueryable<LanguageDto> result = await query.ApplyPipeline(_filter, _projector, cancellationToken);
        return await result.ToLoaderResult(keys, r => r.Code, cancellationToken);
    }
}