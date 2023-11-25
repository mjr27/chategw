using System.Text.Json;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Application.Messaging.Mp3;
using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Commands.Utilities;

/// <summary> Recalc mp3manifest </summary>
[Command("recalc mp3manifest", Description = "Create manifest for book mp3 files")]
public class CreateMp3ManifestCommand : ICommand
{
    private readonly WebApplication _application;

    /// <summary>
    /// Recalc manifest for all books
    /// </summary>
    [CommandParameter(0, Description = "Recalc manifest for all books", IsRequired = false)]
    public bool FullRecalc { get; init; } = false;

    /// <summary>
    /// Completely replace manifest file
    /// </summary>
    [CommandOption("replace", 'r', Description = "Completely replace manifest file", IsRequired = false)]
    public bool ReplaceManifest { get; init; } = false;

    /// <summary>
    /// List of books to process
    /// </summary>
    [CommandOption("books", 'b', Description = "Publications list", IsRequired = false)]
    public List<int> PublicationId { get; init; } = new();

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="application"></param>
    public CreateMp3ManifestCommand(WebApplication application)
    {
        _application = application;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken ct = console.RegisterCancellationHandler();
        using IServiceScope scope = _application.Services.CreateScope();
        PublicationDbContext db = scope.ServiceProvider.GetRequiredService<PublicationDbContext>();


        Dictionary<int, string> books;

        if (FullRecalc)
        {
            books = await db.Publications
                .AsNoTracking()
                .ToDictionaryAsync(r => r.PublicationId, r => r.Title, ct);
        }
        else if (PublicationId.Any())
        {
            books = await db.Publications
                .AsNoTracking()
                .Where(r => PublicationId.Contains(r.PublicationId))
                .OrderBy(r => r.PublicationId)
                .ToDictionaryAsync(r => r.PublicationId, r => r.Title, ct);
            if (PublicationId.Count != books.Count)
            {
                console.ForegroundColor = ConsoleColor.Red;
                await console.Output.WriteLineAsync(
                    $"Book(s) with Id {JsonSerializer.Serialize(PublicationId.Where(r => !books.ContainsKey(r)))} not found");
                console.ResetColor();
                return;
            }
        }
        else
        {
            console.ForegroundColor = ConsoleColor.Red;
            await console.Output.WriteLineAsync($"Set {nameof(FullRecalc)} to True or enter PublicationIds");
            console.ResetColor();
            return;
        }

        Mp3ManifestService manifestService = scope.ServiceProvider.GetRequiredService<Mp3ManifestService>();
        foreach ((int publicationId, string title) in books)
        {
            console.ForegroundColor = ConsoleColor.Green;
            await console.Output.WriteLineAsync($"Create manifest for {publicationId} - {title}");
            await manifestService.UpdateManifest(
                new UpdateMp3ManifestInput { PublicationId = publicationId, ReplaceManifest = ReplaceManifest },
                ct);
        }
    }
}