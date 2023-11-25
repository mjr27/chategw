using ChatEgw.UI.Persistence;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace ChatEgw.UI.Indexer.ConsoleCommands;

[Command("migrate", Description = "Recreates the database")]
public class MigrateConsoleCommand : ConsoleCommandBase
{
    protected override ValueTask Execute(IConsole console, SearchDbContext db,
        CancellationToken cancellationToken)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        return ValueTask.CompletedTask;
    }
}