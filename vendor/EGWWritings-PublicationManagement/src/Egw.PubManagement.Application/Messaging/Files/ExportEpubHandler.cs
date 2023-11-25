using System.Diagnostics;

using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.EpubExport;
using Egw.PubManagement.EpubExport.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;
using Egw.PubManagement.Storage;

using Microsoft.Extensions.Logging;

namespace Egw.PubManagement.Application.Messaging.Files;

/// <summary> Handler for <see cref="ExportEpubInput"/> </summary>
public class ExportEpubHandler : ApplicationCommandHandler<ExportEpubInput>
{
    private readonly ICoverFetcher _coverFetcher;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IStorage _storage;
    private readonly ILogger<ExportEpubHandler> _logger;

    /// <inheritdoc />
    public override async Task Handle(ExportEpubInput request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var id = Guid.NewGuid();
        DateTimeOffset now = Now;
        var exporter = new EpubGenerator(_db, _coverFetcher, _loggerFactory);
        using var folder = new DisposableFolder();
        DirectoryInfo rawFolder = folder.Folder.CreateSubdirectory("raw");
        var epubFile = new FileInfo(Path.Combine(folder.Folder.FullName, $"{request.PublicationId}.epub"));
        await exporter.Create(request.PublicationId, id, now.DateTime, rawFolder, epubFile, cancellationToken);
        _logger.LogInformation("Epub file {EpubFile} created in {Elapsed}", epubFile.FullName, sw.Elapsed);

        string idChunk = (request.PublicationId / 100 * 100).ToString("00000");
        string uri = $"{idChunk}/{request.PublicationId}/{id:N}.epub";
        await using (FileStream f = epubFile.OpenRead())
        {
            await _storage.Write(uri, f, cancellationToken);
        }

        var entity = new ExportedFile(id, request.PublicationId, now)
        {
            Type = ExportTypeEnum.Epub, Uri = new Uri(uri, UriKind.Relative), Size = epubFile.Length, IsMain = false
        };
        _db.PublicationExports.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public ExportEpubHandler(PublicationDbContext db, IClock clock,
        ICoverFetcher coverFetcher,
        IStorageWrapper storageWrapper,
        ILoggerFactory loggerFactory) : base(db, clock)
    {
        _coverFetcher = coverFetcher;
        _loggerFactory = loggerFactory;
        _storage = storageWrapper.Exports;
        _logger = loggerFactory.CreateLogger<ExportEpubHandler>();
    }
}