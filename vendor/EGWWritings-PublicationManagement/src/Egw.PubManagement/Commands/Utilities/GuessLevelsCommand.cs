using System.Text.Json;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Commands.Utilities;

/// <summary>
/// Recalculates TOC
/// </summary>
[Command("recalc levels", Description = "Guess toc levels")]
public class GuessLevelsCommand : ICommand
{
    private readonly WebApplication _application;

    /// <summary>
    /// Dry run
    /// </summary>
    [CommandOption("dry", 'd', Description = "Dry run", IsRequired = false)]
    public bool DryRun { get; init; } = false;

    /// <summary>
    /// Verbose run
    /// </summary>
    [CommandOption("verbose", 'v', Description = "Verbose", IsRequired = false)]
    public bool Verbose { get; init; } = false;

    /// <summary>
    /// Default constructor
    /// </summary>
    public GuessLevelsCommand(WebApplication application)
    {
        _application = application;
    }

    private record CheckResult(int MaxLevel, List<ParaId> Errors);

    /// <inheritdoc />
    // ReSharper disable once CognitiveComplexity
    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken ct = console.RegisterCancellationHandler();
        using IServiceScope scope = _application.Services.CreateScope();
        PublicationDbContext db = scope.ServiceProvider.GetRequiredService<PublicationDbContext>();
        ILogger<GuessLevelsCommand> logger = scope.ServiceProvider.GetRequiredService<ILogger<GuessLevelsCommand>>();

        List<Publication> publications = await db.Publications
            .Include(r => r.Placement)
            .OrderBy(r => r.PublicationId)
            .Where(r => db.PublicationMp3Files.Any(f => f.PublicationId == r.PublicationId))
            .Where(r => r.Placement != null)
            .ToListAsync(cancellationToken: ct);

        var headingLevels = new Dictionary<int, int>();
        var badParaIds = new List<ParaId>();
        foreach (Publication publication in publications)
        {
            CheckResult checkResult = await Check(db, publication, ct);

            if (checkResult.Errors.Any())
            {
                badParaIds.AddRange(checkResult.Errors);
            }
            else
            {
                int defaultType = DbExtensions.GetDefaultHeadingLevel(publication.Type);
                if (publication.Placement?.TocDepth is not null && publication.Placement.TocDepth.Value > defaultType)
                {
                    defaultType = publication.Placement.TocDepth.Value;
                }

                if (checkResult.MaxLevel > defaultType)
                {
                    headingLevels[publication.PublicationId] = checkResult.MaxLevel;
                }
            }

            if (Verbose)
            {
                logger.LogInformation(
                    "For publication {PublicationId} max level is: {Level}",
                    publication.PublicationId,
                    checkResult.MaxLevel);
            }
        }

        await LogBadParagraphs(console, badParaIds);

        if (headingLevels.Any())
        {
            logger.LogInformation("Need to update {Count} publications", headingLevels.Count);
        }

        if (DryRun)
        {
            return;
        }

        foreach ((int publicationId, int level) in headingLevels)
        {
            Publication publication = publications.Single(r => r.PublicationId == publicationId);
            publication.Placement!.TocDepth = level;
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task LogBadParagraphs(IConsole console, List<ParaId> badParaIds)
    {
        if (!badParaIds.Any())
        {
            return;
        }
        var exportFormat = badParaIds.GroupBy(r => r.PublicationId)
            .ToDictionary(r => r.Key, r => r.Distinct()
                .OrderBy(e => e.ElementId)
                .Select(b => b.ToString()).ToArray());
        await console.Output.WriteLineAsync(JsonSerializer.Serialize(exportFormat,
            new JsonSerializerOptions { WriteIndented = true }));
    }

    private static async Task<CheckResult> Check(PublicationDbContext db, Publication publication, CancellationToken ct)
    {
        List<ParaId> mp3Files = await db.PublicationMp3Files
            .Where(r => r.PublicationId == publication.PublicationId)
            .Select(r => r.ParaId)
            .Distinct()
            .ToListAsync(ct);
        var headings = await db.Paragraphs
            .Where(r => r.PublicationId == publication.PublicationId)
            .Where(p => mp3Files.Contains(p.ParaId))
            .Select(r => new { r.ParaId, r.HeadingLevel })
            .ToListAsync(ct);

        var missing = new List<ParaId>();

        if (headings.Count != mp3Files.Count)
        {
            missing.AddRange(mp3Files.Except(headings.Select(r => r.ParaId)));
        }

        missing.AddRange(headings
            .Where(r => r.HeadingLevel is null or 0)
            .Select(r => r.ParaId));

        int maxHeading = missing.Any()
            ? 0
            : headings
                .Where(r => r.HeadingLevel is not (0 or null))
                .Select(r => r.HeadingLevel!.Value)
                .DefaultIfEmpty()
                .Max();

        return new CheckResult(maxHeading, missing);
    }
}