using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Commands.Utilities;

/// <summary>
/// Recalculates TOC
/// </summary>
[Command("recalc folders", Description = "Recalculates Folders")]
public class RecalculateFoldersConsoleCommand : ICommand
{
    private readonly WebApplication _application;


    /// <summary>
    /// Default constructor
    /// </summary>
    public RecalculateFoldersConsoleCommand(WebApplication application)
    {
        _application = application;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken ct = console.RegisterCancellationHandler();
        using IServiceScope scope = _application.Services.CreateScope();
        PublicationDbContext db = scope.ServiceProvider.GetRequiredService<PublicationDbContext>();
        await db.RecalculateFolders(DateTimeOffset.UtcNow, ct);
        await db.Database.ExecuteSqlAsync($"SELECT setval('folders_id_seq', max(id)+2) FROM folders", ct);
    }
}