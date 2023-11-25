using System.Threading.Tasks.Dataflow;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Application.Messaging.Import;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Egw.ImportUtils.Commands;

[Command("recalc", Description = "Recalculates all publications in the database")]
public class RecalcCommand : ICommand
{
    [CommandOption("threads", 't', Description = "Thread count")]
    public int ThreadCount { get; init; } = 1;

    [CommandOption("publications", 'p', Description = "List of publications to recalculate")]
    public int[] Publications { get; init; } = Array.Empty<int>();

    [CommandOption("pub", Description = "Publications connection string",
        EnvironmentVariable = GlobalConstants.PubConnectionStringVariableName)]
    public required string PublicationConnectionString { get; init; }

    private class ClockImpl : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }

    public RecalcCommand()
    {
        _loggerFactory = DatabaseUtilities.CreateLoggerFactory();
        _clock = new ClockImpl();
    }

    private readonly ILoggerFactory _loggerFactory;
    private readonly ClockImpl _clock;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken cancellationToken = console.RegisterCancellationHandler();

        var actionBlock = new ActionBlock<int>(Recalculate, // What to do on each item
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = ThreadCount, EnsureOrdered = false, CancellationToken = cancellationToken
            }); // How many items at the same time

        await using PublicationDbContext db = DatabaseUtilities.ConnectToPubDb(PublicationConnectionString);
        List<int> publications = await db.Publications
            .Select(r => r.PublicationId)
            .ToListAsync(cancellationToken);
        if (Publications.Any())
        {
            publications.RemoveAll(p => !Publications.Contains(p));
        }
        foreach (int publicationId in publications)
        {
            actionBlock.Post(publicationId);
        }

        actionBlock.Complete();
        await actionBlock.Completion;
    }

    private void Recalculate(int publicationId)
    {
        ILogger<RecalcCommand> logger = _loggerFactory.CreateLogger<RecalcCommand>();
        using PublicationDbContext db = DatabaseUtilities.ConnectToPubDb(PublicationConnectionString);
        var recalculateTocHandler = new RecalculateTocPublicationHandler(db, _clock);
        var recalculateMetaHandler = new RecalculatePublicationMetadataHandler(db, _loggerFactory, _clock);
        recalculateMetaHandler.Handle(new RecalculatePublicationMetadataCommand(publicationId), CancellationToken.None).Wait();
        recalculateTocHandler.Handle(new RecalculateTocPublicationCommand(publicationId), CancellationToken.None).Wait();
        logger.LogWarning("Processed publication {Publication}", publicationId);
    }
}