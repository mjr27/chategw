using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.ImportUtils.Services.Importers;

using Microsoft.Extensions.Logging;

namespace Egw.ImportUtils.Commands;

[Command("import exports", Description = "Imports files from EGW Database")]
// ReSharper disable once UnusedType.Global
public class ImportExportsCommand : ICommand
{
    [CommandOption("egw", Description = "EGW Database Connection String",
        EnvironmentVariable = GlobalConstants.EgwConnectionStringVariableName)]
    public required string EgwConnectionString { get; init; }

    [CommandOption("pub", Description = "Publications connection string",
        EnvironmentVariable = GlobalConstants.PubConnectionStringVariableName)]
    public required string PublicationConnectionString { get; init; }

    [CommandOption("uri", 'u', Description = "Root URI for media files")]
    public Uri RootUri { get; init; } = new("https://media2.egwwritings.org");


    [CommandParameter(0, Description = "Output file", IsRequired = true)]
    public required FileInfo OutputFile { get; init; } 

    public async ValueTask ExecuteAsync(IConsole console)
    {
        ILoggerFactory lf = LoggerFactory.Create(o =>
        {
            o.SetMinimumLevel(LogLevel.Information);
            o.AddConsole();
        });
        using var importerService = new ExportImporterService(
            lf.CreateLogger<ExportImporterService>(),
            RootUri,
            OutputFile,
            PublicationConnectionString,
            EgwConnectionString
        );
        await importerService.Import(console.RegisterCancellationHandler());
    }
}