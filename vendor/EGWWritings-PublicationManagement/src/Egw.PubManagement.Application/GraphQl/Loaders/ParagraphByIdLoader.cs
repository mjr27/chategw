using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using GreenDonut;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.GraphQl.Loaders;

/// <summary>
/// Loads <see cref="FolderDto"/> by Id
/// </summary>
public class ParagraphByIdLoader : BatchDataLoader<ParaId, ParagraphDto?>
{
    private readonly IDbContextFactory<PublicationDbContext> _dbContextFactory;
    private readonly IEntityProjector<Paragraph, ParagraphDto> _projector;
    private readonly IEntityPrefilter<Paragraph> _filter;

    /// <inheritdoc />
    public ParagraphByIdLoader(
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
    protected override async Task<IReadOnlyDictionary<ParaId, ParagraphDto?>> LoadBatchAsync(IReadOnlyList<ParaId> keys,
        CancellationToken cancellationToken)
    {
        await using PublicationDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        IQueryable<Paragraph> query = db.Paragraphs.Where(r => keys.Contains(r.ParaId));
        IQueryable<ParagraphDto> result = await query.ApplyPipeline(_filter, _projector, cancellationToken);
        return await result.ToLoaderResult(keys, r => r.ParaId, cancellationToken);
    }
}