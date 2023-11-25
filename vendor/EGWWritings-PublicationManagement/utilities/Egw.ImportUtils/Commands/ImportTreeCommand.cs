using System.Text.Json;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.ImportUtils.Services.Importers;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.LegacyImport;
using Egw.PubManagement.Persistence.Enums;

using Microsoft.Extensions.Logging;
namespace Egw.ImportUtils.Commands;

[Command("import tree", Description = "Imports tree from EGW Database")]
// ReSharper disable once UnusedType.Global
public class ImportTreeCommand : ICommand
{
    [CommandOption("egw", Description = "EGW Database Connection String",
        EnvironmentVariable = GlobalConstants.EgwConnectionStringVariableName)]
    public string EgwConnectionString { get; init; } = "";

    [CommandOption("pub", Description = "Publications connection string",
        EnvironmentVariable = GlobalConstants.PubConnectionStringVariableName)]
    public string PublicationConnectionString { get; init; } = "";

    [CommandOption("config", 'c', Description = "Publication permissions configuration file")]
    public FileInfo PublicationPermissionsFile { get; init; } = new("default-permissions.json");

    private class ClockImpl : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.UnixEpoch;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        Dictionary<int, PublicationPermissionEnum?> configuration;
        await using FileStream f = PublicationPermissionsFile.OpenRead();
        {
            configuration = ReadConfigurationFromStream(f);
        }
        ILoggerFactory lf = DatabaseUtilities.CreateLoggerFactory();

        var detector = new LegacyPublicationTypeDetector();
        using var importerService = new TreeImporterService(
            detector,
            new ClockImpl(),
            lf.CreateLogger<TreeImporterService>(),
            PublicationConnectionString,
            EgwConnectionString,
            configuration
        );
        await importerService.Import(console.RegisterCancellationHandler());
    }

    private static Dictionary<int, PublicationPermissionEnum?> ReadConfigurationFromStream(Stream fileStream)
    {
        var result = new Dictionary<int, PublicationPermissionEnum?>();
        Dictionary<string, List<int>>? json = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(fileStream);
        if (json is null)
        {
            return result;
        }

        foreach ((string permissionType, List<int> valueList) in json)
        {
            PublicationPermissionEnum? permission = permissionType.ToLowerInvariant() switch
            {
                "hidden" => PublicationPermissionEnum.Hidden,
                "authenticated" => PublicationPermissionEnum.AuthenticatedOnly,
                "purchased" => PublicationPermissionEnum.PurchasedOnly,
                "skipped" => null,
                _ => PublicationPermissionEnum.Public
            };
            if (permission == PublicationPermissionEnum.Public)
            {
                continue;
            }

            foreach (int id in valueList)
            {
                result.Add(id, permission);
            }
        }

        return result;
    }
}