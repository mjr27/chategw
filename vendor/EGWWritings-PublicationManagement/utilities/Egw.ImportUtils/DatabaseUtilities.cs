using System.Data;

using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MySqlConnector;
namespace Egw.ImportUtils;

internal static class DatabaseUtilities
{
    internal static PublicationDbContext ConnectToPubDb(string connectionString)
    {
        var db = new PublicationDbContext(new DbContextOptionsBuilder<PublicationDbContext>()
            .UseNpgsql(connectionString, m => m.MigrationsAssembly("Egw.PubManagement"))
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options
        );
        db.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
        return db;
    }

    internal static IDbConnection ConnectToEgwOldDb(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }

    internal static ILoggerFactory CreateLoggerFactory() => LoggerFactory.Create(o =>
    {
        o.SetMinimumLevel(LogLevel.Information);
        o.AddConsole();
    });
}