using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.ImportUtils.Services.Importers;

using Microsoft.Extensions.Logging;
namespace Egw.ImportUtils.Commands;

[Command("import synonyms", Description = "Imports synonyms from EGW Database")]
// ReSharper disable once UnusedType.Global
public class ImportSynonymsCommand : ICommand
{
    [CommandOption("egw", Description = "EGW Database Connection String",
        EnvironmentVariable = GlobalConstants.EgwConnectionStringVariableName)]
    public required string EgwConnectionString { get; init; }

    [CommandOption("pub", Description = "Publications connection string",
        EnvironmentVariable = GlobalConstants.PubConnectionStringVariableName)]
    public required string PublicationConnectionString { get; init; }


    public async ValueTask ExecuteAsync(IConsole console)
    {
        ILoggerFactory lf = LoggerFactory.Create(o =>
        {
            o.SetMinimumLevel(LogLevel.Information);
            o.AddConsole();
        });
        using var importerService = new SynonymImporterService(
            lf.CreateLogger<SynonymImporterService>(),
            PublicationConnectionString,
            EgwConnectionString
        );
        await importerService.Import(console.RegisterCancellationHandler());
    }


}