using System.Diagnostics;

using Dapper;

using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Egw.ImportUtils.Services.Importers;

public class SynonymImporterService : ImporterServiceBase
{
    private readonly ILogger<SynonymImporterService> _logger;

    public SynonymImporterService(
        ILogger<SynonymImporterService> logger,
        string publicationDbConnectionString,
        string egwConnectionString
    ) : base(publicationDbConnectionString, egwConnectionString)
    {
        _logger = logger;
    }

    public override Task Import(CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var availablePublications = _db.PublicationPlacement
            .AsNoTracking()
            .Select(r => r.PublicationId)
            .ToList();
        var publications = _egwDb.Query(SynonymsQuery, new { publications = availablePublications })
            .ToList();
        var swTotal = Stopwatch.StartNew();
        var synonymList = publications
            .Select(row => new PublicationSynonym(row.PublicationId, row.IdElement, row.Synonym, now))
            .ToList();

        _db.ChangeTracker.Clear();
        _db.PublicationSynonyms.ExecuteDelete();
        _db.PublicationSynonyms.AddRange(synonymList);
        _db.SaveChanges();
        _logger.LogError("{Count} publications were processed in {Elapsed}", publications.Count, swTotal.Elapsed);
        return Task.CompletedTask;
    }

    private const string SynonymsQuery = @"-- noinspection SqlDialectInspectionForFile
select pubnr               as PublicationId,
       possible_identifier as Synonym,
       id_element          as IdElement
from sqlb_publicationoverview_possible
where pubnr in @publications
order by pubnr;
";
}