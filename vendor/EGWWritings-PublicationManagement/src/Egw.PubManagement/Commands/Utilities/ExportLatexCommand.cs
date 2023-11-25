using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Application.Services;
using Egw.PubManagement.LatexExport;
using Egw.PubManagement.Services.Latex;

using Path = System.IO.Path;

namespace Egw.PubManagement.Commands.Utilities;

/// <summary> Export epub </summary>
[Command("export latex", Description = "Export latex file")]
public class ExportLatexCommand : ICommand
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
    public FileInfo? Output { get; set; }

    /// <summary> Default constructor </summary>
    public ExportLatexCommand(WebApplication application)
    {
        _application = application;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var options = new LatexPublicationOptions { EndNoteLevel = 1, InsertTocBefore = null };
        CancellationToken ct = console.RegisterCancellationHandler();
        using IServiceScope scope = _application.Services.CreateScope();
        ILogger<ExportLatexCommand> logger = scope.ServiceProvider.GetRequiredService<ILogger<ExportLatexCommand>>();
        var generator = new LatexGenerator(
            scope.ServiceProvider.GetRequiredService<ILatexPublicationRepository>(),
            scope.ServiceProvider.GetRequiredService<ILatexCoverRepository>(),
            Array.Empty<ILatexHeadingTransformer>());
        var exporter = new PdfExportClient(scope.ServiceProvider.GetRequiredService<HttpClient>());
        Output ??= new FileInfo($"{PublicationId}.pdf");

        using var folder = new DisposableFolder();
        logger.LogInformation("Creating folder {Folder}", folder.Folder.FullName);
        var outputEpub = new FileInfo(Path.Combine(folder.Folder.FullName, $"{PublicationId}.tex"));
        var outputJpeg = new FileInfo(Path.ChangeExtension(outputEpub.FullName, ".jpg"));
        await generator.ExportLatex(PublicationId, options, outputEpub, ct);
    
        if (Output.Exists)
        {
            Output.Delete();
        }

        await exporter.ExportPdfFile(Output, outputEpub, outputJpeg, ct);
        if (Output.Exists)
        {
            logger.LogWarning("Export to {Filename} succeeded", Output.FullName);
        }
    }
}