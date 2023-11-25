using System.Globalization;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using CsvHelper;

using Egw.PubManagement.Application.Messaging.Files;
using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Path = System.IO.Path;

namespace Egw.PubManagement.Commands.Utilities;

/// <summary>
/// Migration command
/// </summary>
[Command("import files", Description = "Imports files specified in a  a CSV file")]
public class ImportFilesCommand : ICommand, IDisposable
{
    /// <summary>
    /// List of books to process
    /// </summary>
    [CommandParameter(0, Description = "CSV file to import", IsRequired = true)]
    public FileInfo CsvFile { get; init; } = null!;

    private readonly WebApplication _application;
    private readonly ILogger<ImportFilesCommand> _logger;
    private readonly DisposableFolder _folder;

    /// <summary>
    /// Default constructor
    /// </summary>
    public ImportFilesCommand(WebApplication application)
    {
        _application = application;
        _logger = application.Services.GetRequiredService<ILogger<ImportFilesCommand>>();
        _folder = new DisposableFolder();
    }

    private class FileSystemFile : IFile
    {
        private readonly FileInfo _file;

        public FileSystemFile(FileInfo file)
        {
            _file = file;
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = new())
        {
            await using Stream f = OpenReadStream();
            await f.CopyToAsync(target, cancellationToken);
        }

        public Stream OpenReadStream() => _file.OpenRead();

        public string Name => _file.Name;
        public long? Length => _file.Length;
        public string? ContentType => null;
    }

    private record FileImportTask(Guid Id, int PublicationId, ExportTypeEnum Type, Uri Uri);

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken ct = console.RegisterCancellationHandler();
        List<FileImportTask> queue = await LoadQueue(ct);

        _logger.LogInformation("Loaded {Count} tasks", queue.Count);
        await Parallel.ForEachAsync(queue,
            new ParallelOptions { CancellationToken = ct, MaxDegreeOfParallelism = 8 },
            HandleImportTask
        );
        _logger.LogInformation("Done");
    }

    private async ValueTask HandleImportTask(FileImportTask task, CancellationToken ct)
    {
        using IServiceScope scope = _application.Services.CreateScope();
        using HttpClient httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
        if (task.Type != ExportTypeEnum.Epub)
        {
            return;
        }

        using HttpResponseMessage response = await httpClient.GetAsync(task.Uri, ct);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to download {Uri} for publication {PublicationId}", task.Uri,
                task.PublicationId);
            return;
        }

        var tempFile = new FileInfo(Path.Combine(
            _folder.Folder.FullName,
            Path.GetRandomFileName() + GetExtension(task.Type)
        ));
        await using (FileStream f = tempFile.OpenWrite())
        {
            await response.Content.CopyToAsync(f, ct);
        }

        response.Dispose();
        var file = new FileSystemFile(tempFile);
        var command = new UploadExportInput
        {
            Id = task.Id, PublicationId = task.PublicationId, Type = task.Type, File = file
        };
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(command, ct);
        _logger.LogInformation("Imported {Uri} for publication {PublicationId}", task.Uri, task.PublicationId);
    }

    private static string GetExtension(ExportTypeEnum type) => type switch
    {
        ExportTypeEnum.Epub => ".epub",
        ExportTypeEnum.Mobi => ".mobi",
        ExportTypeEnum.Mp3 => ".zip",
        ExportTypeEnum.Pdf => ".pdf",
        _ => ""
    };

    private async Task<List<FileImportTask>> LoadQueue(CancellationToken cancellationToken)
    {
        var queue = new List<FileImportTask>();
        using IServiceScope scope = _application.Services.CreateScope();
        PublicationDbContext db = scope.ServiceProvider.GetRequiredService<PublicationDbContext>();
        var existingIds =
            new HashSet<Guid>(await db.PublicationExports.Select(r => r.Id).ToListAsync(cancellationToken));
        await using FileStream f = CsvFile.OpenRead();
        using var f2 = new StreamReader(f);
        using var csv = new CsvReader(f2, CultureInfo.InvariantCulture, true);
        await csv.ReadAsync();
        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            Guid id = csv.GetField<Guid>("Id");
            int publicationId = csv.GetField<int>("PublicationId");
            ExportTypeEnum type = Enum.Parse<ExportTypeEnum>(csv.GetField<string>("Type")!);
            Uri uri = new(csv.GetField<string>("Uri")!);
            if (existingIds.Contains(id))
            {
                continue;
            }

            queue.Add(new FileImportTask(id, publicationId, type, uri));
        }

        return queue;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _folder.Dispose();
    }
}