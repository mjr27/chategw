using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using GreenDonut;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Application.GraphQl.Loaders;

/// <summary>
/// Loads <see cref="ParagraphDto"/> by PublicationId last created paragraph
/// </summary>
public class ParagraphByPublicationLoader : BatchDataLoader<int, ParagraphDto?>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IEntityProjector<Paragraph, ParagraphDto> _projector;
    private readonly IEntityPrefilter<Paragraph> _filter;

    /// <inheritdoc />
    public ParagraphByPublicationLoader(
        IDbContextFactory<PublicationDbContext> dbContextFactory,
        IEntityProjector<Paragraph, ParagraphDto> projector,
        IEntityPrefilter<Paragraph> filter,
        IBatchScheduler batchScheduler, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _projector = projector;
        _filter = filter;
    }

    /// <inheritdoc />
    protected override async Task<IReadOnlyDictionary<int, ParagraphDto?>> LoadBatchAsync(IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        IQueryable<Paragraph> query = db.Paragraphs
            .Where(r => keys.Contains(r.PublicationId))
            .OrderByDescending(r => r.ParagraphId)
            .Take(1);
        IQueryable<ParagraphDto> result = await query.ApplyPipeline(_filter, _projector, cancellationToken);

        return await result.ToLoaderResult(keys, r => r.PublicationId, cancellationToken);
    }
}