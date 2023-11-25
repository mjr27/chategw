using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Application.Messaging.Import;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

using ShellProgressBar;

namespace Egw.PubManagement.Commands.Utilities;

/// <summary>
/// Recalculates TOC
/// </summary>
[Command("recalc toc", Description = "Recalculates TOC")]
public class RecalculateTocCommand : ICommand
{
    private readonly WebApplication _application;

    /// <summary>
    /// List of books to process
    /// </summary>
    [CommandOption("book", 'b', Description = "List of books to process", IsRequired = false)]
    public int[] Books { get; init; } = Array.Empty<int>();


    /// <summary>
    /// Default constructor
    /// </summary>
    public RecalculateTocCommand(WebApplication application)
    {
        _application = application;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken ct = console.RegisterCancellationHandler();
        using IServiceScope scope = _application.Services.CreateScope();
        PublicationDbContext db = scope.ServiceProvider.GetRequiredService<PublicationDbContext>();
        IRequestHandler<RecalculateTocPublicationCommand> handler = scope.ServiceProvider
            .GetRequiredService<IRequestHandler<RecalculateTocPublicationCommand>>();
        IQueryable<Publication> publicationsQuery = db.Publications
            .OrderBy(r => r.Placement!.Folder.GlobalOrder)
            .ThenBy(r => r.Placement!.Order);
        if (Books.Length > 0)
        {
            publicationsQuery = publicationsQuery.Where(r => Books.Contains(r.PublicationId));
        }

        var publications = await publicationsQuery
            .Select(r => new { r.PublicationId, r.Title })
            .ToListAsync(ct);

        using var progress = new ProgressBar(publications.Count, "Processing TOC");
        foreach (var publication in publications)
        {
            progress.Tick($"Processing {publication.Title} #{publication.PublicationId}");
            await handler.Handle(new RecalculateTocPublicationCommand(publication.PublicationId), ct);
        }
    }
}