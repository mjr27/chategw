using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;

using Egw.ImportUtils.Services;
using Egw.PubManagement.Application.Services;
using Egw.PubManagement.LegacyImport;
using Egw.PubManagement.LegacyImport.LinkRepository;
using Egw.PubManagement.Persistence;

using Microsoft.Extensions.Logging;
namespace Egw.ImportUtils.Commands;

[Command("legacy convert", Description = "Converts multiple publications to the new format")]
public class LegacyConvertCommand : ICommand
{
    private const string SourceExtension = "html";

    private readonly ILoggerFactory _lf;
    private readonly ILogger<LegacyConvertCommand> _logger;

    [CommandOption("cache", 'c', Description = "Json Cache file name")]
    public FileInfo CacheFile { get; init; } = new("cache.json");

    [CommandOption("threads", 't', Description = "Thread count")]
    public int ThreadCount { get; init; } = 1;

    [CommandOption("warnings", 'w', Description = "Allow warnings")]
    public bool AllowWarnings { get; init; } = false;


    [CommandParameter(0, Description = "Source files or folders with html files")]
    public string[] Sources { get; init; } = Array.Empty<string>();

    [CommandOption("output", 'o', Description = "Destination folder")]
    public required DirectoryInfo DestinationFolder { get; init; }

    [CommandOption("pub", Description = "Publications connection string",
        EnvironmentVariable = GlobalConstants.PubConnectionStringVariableName)]
    public required string PublicationConnectionString { get; init; }

    public LegacyConvertCommand()
    {
        _lf = DatabaseUtilities.CreateLoggerFactory();
        _logger = _lf.CreateLogger<LegacyConvertCommand>();
    }

    private ILinkRepository? _repo;
    private readonly object _repoLock = new();
    private ILinkRepository Repo
    {
        get
        {
            if (_repo is not null)
            {
                return _repo;
            }

            lock (_repoLock)
            {
                if (_repo is not null)
                {
                    return _repo;
                }

                using PublicationDbContext pubDb = DatabaseUtilities.ConnectToPubDb(PublicationConnectionString);
                _repo = new DatabaseJsonLinkRepository(pubDb);
                return _repo;
            }
        }
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken cancellationSource = console.RegisterCancellationHandler();

        if (!DestinationFolder.Exists)
        {
            DestinationFolder.Create();
        }

        var actionBlock = new ActionBlock<FileDetails>(Convert, // What to do on each item
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = ThreadCount, EnsureOrdered = false, CancellationToken = cancellationSource
            }); // How many items at the same time

        foreach (FileDetails file in MakeSourceList())
        {
            actionBlock.Post(file);
        }

        actionBlock.Complete();
        await actionBlock.Completion;
    }

    private List<FileDetails> MakeSourceList()
    {
        var sources = new List<FileDetails>();
        foreach (string source in Sources)
        {
            if (File.Exists(source))
            {
                FileDetails? task = MakeTask(new FileInfo(source));
                if (task != null)
                {
                    sources.Add(task);
                }
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

    private void Convert(FileDetails file)
    {
        var converter = new PublicationConverter(_lf, Repo);
        try
        {
            var sw = Stopwatch.StartNew();
            converter.Convert(file.InputFile, file.OutputFile, true, AllowWarnings ? WarningLevel.Error : WarningLevel.Warning);
            _logger.LogInformation("Converted {Filename} in {Elapsed}", file.InputFile, sw.Elapsed);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error converting {Filename}", file.InputFile);
        }
    }

    private IEnumerable<FileDetails> EnumerateFiles(DirectoryInfo folder)
    {
        return EnumerateFilesInternal(folder)
            .OrderBy(r =>
            {
                string f = Path.ChangeExtension(Path.GetFileName(r.InputFile), null);
                return !int.TryParse(f, out int result) ? 0 : result;
            });
    }

    private IEnumerable<FileDetails> EnumerateFilesInternal(DirectoryInfo folder)
    {
        foreach (FileInfo sourceFile in folder.EnumerateFiles($"*.{SourceExtension}"))
        {
            FileDetails? task = MakeTask(sourceFile);
            if (task != null)
            {
                yield return task;
            }
        }
    }

    private FileDetails? MakeTask(FileInfo sourceFile)
    {
        string targetFile = Path.ChangeExtension(sourceFile.Name, ".html");
        targetFile = Path.Combine(DestinationFolder.FullName, targetFile);
        if (!File.Exists(targetFile))
        {
            return new FileDetails(sourceFile.FullName, targetFile);
        }

        _logger.LogInformation("File {Filename} already exists, skipping", targetFile);
        return null;
    }

    private record FileDetails(string InputFile, string OutputFile);

}