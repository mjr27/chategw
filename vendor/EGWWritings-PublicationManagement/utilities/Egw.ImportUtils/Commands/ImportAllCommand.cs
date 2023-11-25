using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Egw.ImportUtils.Commands;

[Command("import", Description = "Imports the whole initial data into the database")]
// ReSharper disable once UnusedType.Global
public class ImportAllCommand : ICommand
{
    [CommandOption("egw", Description = "EGW Database Connection String",
        EnvironmentVariable = GlobalConstants.EgwConnectionStringVariableName)]
    public string EgwConnectionString { get; init; } = "";

    [CommandOption("pub", Description = "Publications connection string",
        EnvironmentVariable = GlobalConstants.PubConnectionStringVariableName)]
    public string PublicationConnectionString { get; init; } = "";

    [CommandOption("config", 'c', Description = "Publication permissions configuration file")]
    public FileInfo PublicationPermissionsFile { get; init; } = new FileInfo("default-permissions.json");

    [CommandOption("uri", 'u', Description = "Root URI for media files", IsRequired = false)]
    public Uri RootUri { get; init; } = new("https://media2.egwwritings.org");

    public async ValueTask ExecuteAsync(IConsole console)
    {
        await using (PublicationDbContext db = DatabaseUtilities.ConnectToPubDb(PublicationConnectionString))
        {
            await db.Database.MigrateAsync();
        }

        await new ImportTreeCommand
        {
            EgwConnectionString = EgwConnectionString,
            PublicationConnectionString = PublicationConnectionString,
            PublicationPermissionsFile = PublicationPermissionsFile
        }.ExecuteAsync(console);
        await new ImportExportsCommand
        {
            EgwConnectionString = EgwConnectionString,
            PublicationConnectionString = PublicationConnectionString,
            OutputFile = new FileInfo("exported-files.csv"),
            RootUri = RootUri
        }.ExecuteAsync(console);
        await new ImportMp3MarkupCommand
        {
            EgwConnectionString = EgwConnectionString, PublicationConnectionString = PublicationConnectionString,
        }.ExecuteAsync(console);
        await new ImportPubLinksCommand
        {
            EgwConnectionString = EgwConnectionString, PublicationConnectionString = PublicationConnectionString,
        }.ExecuteAsync(console);
        await new ImportSynonymsCommand
        {
            EgwConnectionString = EgwConnectionString, PublicationConnectionString = PublicationConnectionString,
        }.ExecuteAsync(console);
    }
}