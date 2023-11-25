using Egw.PubManagement.Application.Services.Metadata.Builders;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using EnumsNET;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Application.Services;

internal class MetadataGenerator
{
    private readonly PublicationDbContext _db;
    private readonly ILogger<MetadataGenerator> _logger;
    private readonly Dictionary<PublicationType, IMetadataBuilder> _metadataBuilders;

    public MetadataGenerator(PublicationDbContext db, ILogger<MetadataGenerator> logger)
    {
        _db = db;
        _logger = logger;
        _metadataBuilders = new Dictionary<PublicationType, IMetadataBuilder>
        {
            [PublicationType.Book] = new BookMetadataBuilder(),
            [PublicationType.Bible] = new BibleMetadataBuilder(),
            [PublicationType.BibleCommentary] = new BibleCommentaryMetadataBuilder(),
            [PublicationType.Devotional] = new DevotionalMetadataBuilder(),
            [PublicationType.Dictionary] = new DictionaryMetadataBuilder(),
            [PublicationType.Manuscript] = new LtMsMetadataBuilder(),
            [PublicationType.PeriodicalNoPageBreak] = new PeriodicalNoPageBreakMetadataBuilder(),
            [PublicationType.PeriodicalPageBreak] = new PeriodicalPageBreakMetadataBuilder(),
            [PublicationType.ScriptureIndex] = new ScriptureIndexMetadataBuilder(),
            [PublicationType.TopicalIndex] = new TopicalIndexMetadataBuilder(),
        };
    }

    public async Task FillPublicationMetadata(int bookId, DateTimeOffset currentDate,
        CancellationToken cancellationToken)
    {
        Publication? publication = await _db.Publications
            .AsNoTracking()
            .Where(r => r.PublicationId == bookId).FirstOrDefaultAsync(cancellationToken);

        if (publication is null)
        {
            _logger.LogWarning("Publication {Id} not found", bookId);
            return;
        }

        if (!_metadataBuilders.TryGetValue(publication.Type, out IMetadataBuilder? metadataBuilder))
        {
            _logger.LogError("Publication {PublicationId} not found (type is {PublicationType})",
                publication.PublicationId,
                publication.Type.AsString(EnumFormat.Description) ?? "null"
            );
            return;
        }

        List<Paragraph> paragraphs = await _db.Paragraphs
            .AsNoTracking()
            .OrderBy(r => r.Order)
            .Where(r => r.PublicationId == bookId)
            .ToListAsync(cancellationToken);

        await _db.ParagraphMetadata.Where(r => r.PublicationId == bookId).ExecuteDeleteAsync(cancellationToken);
        IEnumerable<ParagraphMetadata> metadata = metadataBuilder.GetMetadata(publication, paragraphs, currentDate)
            .Where(r => r.Pagination != null || r.Date != null || r.LtMsMetadata != null || r.BibleMetadata != null);
        await _db.ChunkedInsertAsync(metadata, cancellationToken);
    }
}