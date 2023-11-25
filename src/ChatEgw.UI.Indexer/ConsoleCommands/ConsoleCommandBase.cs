using ChatEgw.UI.Persistence;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Pgvector.EntityFrameworkCore;

namespace ChatEgw.UI.Indexer.ConsoleCommands;

public abstract class ConsoleCommandBase : ICommand
{
    protected readonly ILoggerFactory LoggingFactory;

    [CommandOption(
        "database", 'd', Description = "Postgres database to use", IsRequired = true,
        EnvironmentVariable = "EGW_SEARCH_DSN"
    )]
    public required string DatabaseDsn { get; init; }

    protected ConsoleCommandBase()
    {
        LoggingFactory = LoggerFactory.Create(o => o.AddSimpleConsole()
            .AddFilter((cat, level) =>
            {
                if (cat is null)
                {
                    return level >= LogLevel.Information;
                }

                if (cat.StartsWith("Microsoft.EntityFrameworkCore"))
                {
                    return level >= LogLevel.Warning;
                }

                return level >= LogLevel.Information;
            })
        );
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        await using var db =
            new SearchDbContext(CreateDbContext<SearchDbContext>(DatabaseDsn, null, o => o.UseVector()));
        await Execute(console, db, console.RegisterCancellationHandler());
    }

    protected abstract ValueTask Execute(IConsole console, SearchDbContext db,
        CancellationToken cancellationToken);

    protected DbContextOptions<T> CreateDbContext<T>(string dsn,
        Action<DbContextOptionsBuilder>? configure = null,
        Action<NpgsqlDbContextOptionsBuilder>? configureNpgsql = null
    )
        where T : DbContext
    {
        DbContextOptionsBuilder<T> builder = new DbContextOptionsBuilder<T>()
            .EnableDetailedErrors()
            .UseNpgsql(dsn, o =>
            {
                o.CommandTimeout(3600);
                configureNpgsql?.Invoke(o);
            })
            .UseLoggerFactory(LoggingFactory)
            .EnableSensitiveDataLogging();
        configure?.Invoke(builder);
        return builder.Options;
    }
}