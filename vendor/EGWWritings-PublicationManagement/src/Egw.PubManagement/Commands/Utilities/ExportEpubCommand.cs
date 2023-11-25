using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Application.Services;
using Egw.PubManagement.EpubExport;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Services;

using Microsoft.EntityFrameworkCore;

using Path = System.IO.Path;

namespace Egw.PubManagement.Commands.Utilities;

/// <summary> Export epub </summary>
[Command("export epub", Description = "Export ePub file")]
public class ExportEpubCommand : ICommand
{
    private readonly WebApplication _application;

    /// <summary>
    /// List of books to process
    /// </summary>
    [CommandParameter(0, Description = "Publication to export", IsRequired = true)]
    public int PublicationId { get; init; }

    /// <summary>
    /// Output filename
    /// </summary>
    [CommandOption("output", 'o', Description = "Output file", IsRequired = false)]
    public FileInfo? OutputFile { get; set; }

    /// <summary> Default constructor </summary>
    public ExportEpubCommand(WebApplication application)
    {
        _application = application;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken ct = console.RegisterCancellationHandler();
        using IServiceScope scope = _application.Services.CreateScope();
        PublicationDbContext db = scope.ServiceProvider.GetRequiredService<PublicationDbContext>();
        using var folder = new DisposableFolder();
        DirectoryInfo temporaryFolder = folder.Folder.CreateSubdirectory("raw");
        OutputFile ??= new FileInfo($"{PublicationId}.epub");
        var outputEpub = new FileInfo(Path.Combine(folder.Folder.FullName, $"{PublicationId}.epub"));
        var converter = new EpubGenerator(db,
            new CoverFetcher(
                scope.ServiceProvider.GetRequiredService<IStorageWrapper>(),
                scope.ServiceProvider.GetRequiredService<IDbContextFactory<PublicationDbContext>>()
            ),
            scope.ServiceProvider.GetRequiredService<ILoggerFactory>());
        await converter.Create(
            PublicationId,
            Guid.NewGuid(),
            DateTime.UtcNow,
            temporaryFolder, outputEpub, ct);
        await console.Output.WriteLineAsync("EPub saved to: " + outputEpub.FullName);
        outputEpub.CopyTo(OutputFile.FullName, true);
    }
}