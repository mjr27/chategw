using System.Threading.Tasks.Dataflow;

using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;

using Egw.PubManagement.Application.Messaging.Import;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;

using Microsoft.Extensions.Logging;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Serialization;
namespace Egw.ImportUtils.Commands;

[Command("load", Description = "Loads multiple weml files to the database")]
public class LoadCommand : ICommand
{
    private const string SourceExtension = "html";

    [CommandParameter(0, Description = "Source files or folders with html files")]
    public string[] Sources { get; init; } = Array.Empty<string>();
    [CommandOption("threads", 't', Description = "Thread count")]
    public int ThreadCount { get; init; } = 1;
    
    [CommandOption("recalc", 'r', Description = "Recalculate on import")]
    public bool Recalculate { get; init; } = false;
    
    
    [CommandOption("force", 'f', Description = "Force overwrite existing")]
    public bool Force { get; init; } = false;

    [CommandOption("pub", Description = "Publications connection string",
        EnvironmentVariable = GlobalConstants.PubConnectionStringVariableName)]
    public required string PublicationConnectionString { get; init; }

    private class ClockImpl : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }

    public LoadCommand()
    {
        _loggerFactory = DatabaseUtilities.CreateLoggerFactory();
        _deserializer = new WemlDeserializer();
        _clock = new ClockImpl();
    }

    private readonly WemlDeserializer _deserializer;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ClockImpl _clock;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken cancellationSource = console.RegisterCancellationHandler();

        var actionBlock = new ActionBlock<FileInfo>(Load, // What to do on each item
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = ThreadCount, EnsureOrdered = false, CancellationToken = cancellationSource
            }); // How many items at the same time

        foreach (FileInfo file in MakeSourceList())
        {
            actionBlock.Post(file);
        }

        actionBlock.Complete();
        await actionBlock.Completion;
    }

    private List<FileInfo> MakeSourceList()
    {
        var sources = new List<FileInfo>();
        foreach (string source in Sources)
        {
            if (File.Exists(source))
            {
                sources.Add(new FileInfo(source));
            }
            else if (Directory.Exists(source))
            {
                sources.AddRange(EnumerateFiles(new DirectoryInfo(source)));
            }
            else
            {
                throw new CommandException($"source {source} does not exist");
            }
        }

        return sources;
    }

    private void Load(FileInfo file)
    {
        ILogger<LoadCommand> logger = _loggerFactory.CreateLogger<LoadCommand>();
        var parser = new HtmlParser();
        using PublicationDbContext db = DatabaseUtilities.ConnectToPubDb(PublicationConnectionString);
        var handler = new LoadDocumentHandler(
            db,
            _loggerFactory,
            _clock
        );

        using Stream f = file.OpenRead();
        IHtmlDocument htmlDocument = parser.ParseDocument(f);
        WemlDocument wemlDocument = _deserializer.Deserialize(htmlDocument);
        var command = new LoadDocumentCommand(wemlDocument, Recalculate, !Force);
        handler.Handle(command, CancellationToken.None).Wait();
        logger.LogWarning("Loaded {File}", file.Name);
    }

    private IEnumerable<FileInfo> EnumerateFiles(DirectoryInfo folder)
    {
        return EnumerateFilesInternal(folder)
            .OrderBy(r =>
            {
                string f = Path.ChangeExtension(Path.GetFileName(r.Name), null);
                return !int.TryParse(f, out int result) ? 0 : result;
            });
    }

    private static IEnumerable<FileInfo> EnumerateFilesInternal(DirectoryInfo folder)
    {
        return folder.EnumerateFiles($"*.{SourceExtension}");
    }
}