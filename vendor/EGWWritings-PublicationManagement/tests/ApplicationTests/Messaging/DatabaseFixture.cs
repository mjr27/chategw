using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace ApplicationTests.Messaging;

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        Db = CreateContext();
        lock (Lock)
        {
            if (s_created)
            {
                return;
            }

            Db.Database.EnsureDeleted();
            Db.Database.EnsureCreated();
            s_created = true;
        }
    }

    public PublicationDbContext Db { get; set; }
    private static bool s_created;
    private static readonly object Lock = new();

    private PublicationDbContext CreateContext()
    {
        IConfiguration configuration = InitConfiguration();
        DbContextOptionsBuilder<PublicationDbContext> pool = new DbContextOptionsBuilder<PublicationDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Publications"))
            .EnableSensitiveDataLogging();
        return new PublicationDbContext(pool.Options);
    }

    public void Dispose()
    {
        Db.Dispose();
    }

    private static IConfiguration InitConfiguration() => new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

}