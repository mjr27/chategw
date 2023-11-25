using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Commands;

/// <summary>
/// Migration command
/// </summary>
[Command("migrate", Description = "Migrate the database to the latest version")]
public class MigrateCommand : ICommand
{
    private readonly WebApplication _application;

    /// <summary>
    /// Default constructor
    /// </summary>
    public MigrateCommand(WebApplication application)
    {
        _application = application;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken ct = console.RegisterCancellationHandler();
        using IServiceScope scope = _application.Services.CreateScope();
        PublicationDbContext db = scope.ServiceProvider.GetRequiredService<PublicationDbContext>();
        await db.Database.MigrateAsync(ct);
    }
}