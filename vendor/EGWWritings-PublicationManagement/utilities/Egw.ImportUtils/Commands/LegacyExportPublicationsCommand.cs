using System.Data;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.ImportUtils.Services;
using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;
namespace Egw.ImportUtils.Commands;

[Command("legacy export", Description = "Exports publications from legacy database")]
public class LegacyExportPublicationsCommand : ICommand
{
    [CommandOption("egw", Description = "EGW Database Connection String",
        EnvironmentVariable = GlobalConstants.EgwConnectionStringVariableName)]
    public required string EgwConnectionString { get; init; }

    [CommandOption("pub", Description = "Publications connection string",
        EnvironmentVariable = GlobalConstants.PubConnectionStringVariableName)]
    public required string PublicationConnectionString { get; init; }

    [CommandOption("skip", 's', Description = "Skips existing")]
    public bool Skip { get; init; } = false;
    [CommandOption("threads", 't', Description = "Thread count")]
    public int ThreadCount { get; init; } = 1;

    [CommandOption("min", Description = "Minimal publication id")]
    public int MinPublicationId { get; init; } = int.MinValue;
    [CommandOption("max", Description = "Maximal publication id")]
    public int MaxPublicationId { get; init; } = int.MaxValue;

    [CommandParameter(0, Description = "Target directory")]
    public required DirectoryInfo OutputDirectory { get; init; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        PublicationDbContext db = DatabaseUtilities.ConnectToPubDb(PublicationConnectionString);
        CancellationToken cancellationToken = console.RegisterCancellationHandler();
        List<int> availableBooks = await db.PublicationPlacement
            .Select(r => r.PublicationId)
            .Where(r => r >= MinPublicationId && r <= MaxPublicationId)
            .ToListAsync(cancellationToken);
        if (!OutputDirectory.Exists)
        {
            OutputDirectory.Create();
        }

        var actionBlock = new ActionBlock<int>(ExportPublication, // What to do on each item
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = ThreadCount, EnsureOrdered = false, CancellationToken = cancellationToken
            }); // How many items at the same time

        foreach (int pubId in availableBooks)
        {
            actionBlock.Post(pubId);
        }

        actionBlock.Complete();
        await actionBlock.Completion;
    }

    private async Task ExportPublication(int pubId)
    {
        using IDbConnection egwConnection = DatabaseUtilities.ConnectToEgwOldDb(EgwConnectionString);
        var exporter = new BookExporter();
        string path = Path.Combine(OutputDirectory.FullName, $"{pubId}.html");
        if (Skip && File.Exists(path))
        {
            Console.WriteLine($"File {path} already exists");
            return;
        }

        var sw = Stopwatch.StartNew();
        using var f = new MemoryStream();
        await using var writer = new StreamWriter(f);
        await exporter.ExportPublication(egwConnection, pubId, writer);
        await writer.FlushAsync();
        try
        {
            await using FileStream g = File.OpenWrite(path);
            g.Write(f.ToArray());
            g.Flush();
        }
        catch
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            throw;
        }

        Console.WriteLine($"File {path} exported in {sw.ElapsedMilliseconds}ms");
    }
}