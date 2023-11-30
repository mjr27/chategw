using ChatEgw.UI.Indexer.Indexer;
using ChatEgw.UI.Persistence;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Egw.PubManagement.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ChatEgw.UI.Indexer.ConsoleCommands;

[Command("import egw", Description = "Imports english publications from EGW Writings database")]
public class ImportEgwConsoleCommand : ConsoleCommandBase
{
    [CommandOption(
        "from", 'f', Description = "Database to convert from", IsRequired = true,
        EnvironmentVariable = "EGW_EXPORT_FROM"
    )]
    public required string FromDsn { get; init; }

    [CommandOption("batch-size", 'b', Description = "Batch size", IsRequired = false)]
    public int BatchSize { get; init; } = 10_000;

    [Obsolete("Obsolete")]
    public ImportEgwConsoleCommand()
    {
        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
    }

    protected override async ValueTask Execute(IConsole console, SearchDbContext db,
        CancellationToken cancellationToken)
    {
        await using var fromContext = new PublicationDbContext(
            CreateDbContext<PublicationDbContext>(
                FromDsn, o => { o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); }
            )
        );
        var service = new IndexExporterService(fromContext);
        List<SearchNode> nodes = service.GetNodes();
        await db.Paragraphs.ExecuteDeleteAsync(cancellationToken);
        await db.Nodes.ExecuteDeleteAsync(cancellationToken);
        db.Nodes.AddRange(nodes);
        await db.SaveChangesAsync(cancellationToken);
        db.ChangeTracker.Clear();

        var egwPublicationStatus = nodes.Where(r => !r.IsFolder)
            .ToDictionary(r => int.Parse(r.Id[1..]), r => r.IsEgw);
        db.ChangeTracker.AutoDetectChangesEnabled = false;
        foreach (SearchParagraph[] paragraphs in service.FetchParagraphs(egwPublicationStatus).Chunk(BatchSize))
        {
            db.Paragraphs.AddRange(paragraphs);
            await db.SaveChangesAsync(cancellationToken);
        }

        db.ChangeTracker.AutoDetectChangesEnabled = true;
    }
}