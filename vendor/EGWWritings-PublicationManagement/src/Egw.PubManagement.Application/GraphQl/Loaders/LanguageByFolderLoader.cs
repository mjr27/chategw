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
public class LanguageByFolderLoader : BatchDataLoader<int, LanguageDto?>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IEntityProjector<Language, LanguageDto> _projector;
    private readonly IEntityPrefilter<Language> _filter;

    /// <inheritdoc />
    public LanguageByFolderLoader(
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
    protected override async Task<IReadOnlyDictionary<int, LanguageDto?>> LoadBatchAsync(IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        List<Folder> folders = await db.Folders.Where(r => keys.Contains(r.Id))
            .ToListAsync(cancellationToken);
        IQueryable<LanguageDto> languageQuery = await db.Languages
            .Where(r => r.RootFolderId != null)
            .ApplyPipeline(_filter, _projector, cancellationToken);
        Dictionary<int, LanguageDto> languages = await languageQuery
            .ToDictionaryAsync(r => r.RootFolderId!.Value, r => r, cancellationToken);
        var result = keys.ToDictionary(r => r, _ => (LanguageDto?)null);
        foreach (Folder folder in folders)
        {
            int rootFolderId = int.Parse(folder.MaterializedPath[..Folder.MaterializedPathElementLength]);
            result[folder.Id] = languages.GetValueOrDefault(rootFolderId);
        }

        return result;
    }
}