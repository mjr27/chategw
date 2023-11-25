using System.Data;
using System.Text.Json;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.ImportUtils.Services;
using Egw.PubManagement.Application.Models.Internal;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using Z.EntityFramework.Plus;
namespace Egw.ImportUtils.Commands;

[Command("legacy collect-cache", Description = "Collects cache from EGW Database")]
public class LegacyCollectCacheCommand : ICommand
{
    [CommandOption("egw", Description = "EGW Database Connection String",
        EnvironmentVariable = GlobalConstants.EgwConnectionStringVariableName)]
    public required string EgwConnectionString { get; init; }

    [CommandOption("pub", Description = "Publications connection string",
        EnvironmentVariable = GlobalConstants.PubConnectionStringVariableName)]
    public required string PublicationConnectionString { get; init; }

    [CommandOption("cache", 'c', Description = "Json Cache file name")]
    public FileInfo? CacheFile { get; init; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        IDbConnection db = DatabaseUtilities.ConnectToEgwOldDb(EgwConnectionString);
        PublicationDbContext pubDb = DatabaseUtilities.ConnectToPubDb(PublicationConnectionString);
        var publications = pubDb.PublicationPlacement
            .Select(r => r.PublicationId)
            .ToList();

        var storage = new LinkStorage();
        var builder = new LinkStorageBuilder(storage);
        builder.CollectLanguages(db);
        builder.CollectLinks(db, publications);

        if (CacheFile is null)
        {
            using JsonDocument doc = JsonSerializer.SerializeToDocument(storage);
            pubDb.Configuration.Where(r => r.Key == GlobalOptionTypeEnum.LinkCache).Delete();
            using var configurationRow = new GlobalOption(GlobalOptionTypeEnum.LinkCache, doc, DateTimeOffset.UtcNow);
            pubDb.Configuration.Add(configurationRow);
            pubDb.SaveChanges();
        }
        else
        {
            using FileStream f = CacheFile.OpenWrite();
            JsonSerializer.Serialize(f, storage);
        }

        return ValueTask.CompletedTask;
    }
}