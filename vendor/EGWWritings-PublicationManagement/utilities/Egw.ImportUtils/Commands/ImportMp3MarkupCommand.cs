using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.ImportUtils.Services.Importers;

using Microsoft.Extensions.Logging;
namespace Egw.ImportUtils.Commands;

[Command("import mp3", Description = "Imports mp3 files from EGW Database")]
// ReSharper disable once UnusedType.Global
public class ImportMp3MarkupCommand : ICommand
{
    [CommandOption("egw", Description = "EGW Database Connection String",
        EnvironmentVariable = GlobalConstants.EgwConnectionStringVariableName)]
    public required string EgwConnectionString { get; init; }

    [CommandOption("pub", Description = "Publications connection string",
        EnvironmentVariable = GlobalConstants.PubConnectionStringVariableName)]
    public required string PublicationConnectionString { get; init; }

    [CommandOption("mp3", Description = "Mp3 data uri")]
    public string Mp3Uri { get; init; } = "https://media4.egwwritings.org/mp3/mp3data-20230609.json";

    public async ValueTask ExecuteAsync(IConsole console)
    {
        ILoggerFactory lf = LoggerFactory.Create(o =>
        {
            o.SetMinimumLevel(LogLevel.Information);
            o.AddConsole();
        });
        using var importerService = new Mp3ImporterService(
            lf.CreateLogger<Mp3ImporterService>(),
            PublicationConnectionString,
            EgwConnectionString,
            Mp3Uri
        );
        await importerService.Import(console.RegisterCancellationHandler());
    }

}