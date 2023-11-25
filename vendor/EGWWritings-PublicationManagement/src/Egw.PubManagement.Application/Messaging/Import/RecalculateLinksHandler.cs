using AngleSharp;
using AngleSharp.Dom;

using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.LegacyImport.BibleCodes;
using Egw.PubManagement.LegacyImport.LinkRepository;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Links;

namespace Egw.PubManagement.Application.Messaging.Import;

/// <summary> Handler for <see cref="RecalculateLinksCommand"/> </summary>
public class RecalculateLinksHandler : ApplicationCommandHandler<RecalculateLinksCommand>
{
    private readonly ILinkRepository _linkRepository;
    private readonly ILogger<RecalculateLinksHandler> _logger;
    private readonly BibleLinkNormalizer _normalizer;

    /// <inheritdoc />
    // ReSharper disable once CognitiveComplexity
    public override async Task Handle(RecalculateLinksCommand request, CancellationToken cancellationToken)
    {
        var bibles = _db.Publications.Where(r => r.Type == PublicationType.Bible)
            .Select(r => r.PublicationId)
            .ToHashSet();
        Publication publication =
            await _db.Publications.FirstOrDefaultAsync(r => r.PublicationId == request.PublicationId,
                cancellationToken)
            ?? throw new NotFoundProblemDetailsException("Publication not found");

        IAsyncEnumerable<Paragraph> paragraphs =
            _db.Paragraphs
                .Where(r => r.PublicationId == request.PublicationId)
                .OrderBy(r => r.Order)
                .Where(r => r.Content.Contains("egw://"))
                .AsAsyncEnumerable();
        string? normalizedLanguage = _linkRepository.NormalizeLanguage(publication.LanguageCode);
        if (normalizedLanguage is null)
        {
            _logger.LogWarning("Unknown language {Language}", publication.LanguageCode);
            return;
        }

        await foreach (Paragraph paragraph in paragraphs.WithCancellation(cancellationToken))
        {
            IElement root = paragraph.DeserializeToHtml();
            IElement[] links = root.DescendentsAndSelf()
                .OfType<IElement>()
                .Where(r => r.NodeName == "A" && r.HasAttribute("href") && r.HasAttribute("title"))
                .ToArray();
            if (links.Length == 0)
            {
                continue;
            }

            bool changed = false;
            foreach (IElement link in links)
            {
                string? uri = link.GetAttribute("href");
                string? title = link.GetAttribute("title");
                if (string.IsNullOrWhiteSpace(title))
                {
                    continue;
                }

                string? newLink = _linkRepository.FindParagraph(_normalizer.NormalizeRefCode(title), normalizedLanguage);

                if (newLink is null)
                {
                    continue;
                }

                var paraId = ParaId.Parse(newLink);
                var egwLink = new WemlEgwLink(paraId, bibles.Contains(paraId.PublicationId), null);
                if (egwLink.Reference == uri)
                {
                    continue;
                }

                changed = true;
                link.SetAttribute("href", egwLink.Reference);
            }

            if (!changed)
            {
                continue;
            }

            string newContent = root.ToHtml();
            if (paragraph.Content == newContent)
            {
                continue;
            }

            _logger.LogInformation("Updating paragraph {ParaId}", paragraph.ParaId);
            paragraph.ChangeContent(newContent, paragraph.HeadingLevel, paragraph.IsReferenced, Now);
        }

        if (_db.ChangeTracker.HasChanges())
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public RecalculateLinksHandler(PublicationDbContext db, IClock clock,
        ILinkRepository linkRepository,
        ILogger<RecalculateLinksHandler> logger) :
        base(db, clock)
    {
        _linkRepository = linkRepository;
        _logger = logger;
        _normalizer = new BibleLinkNormalizer();
    }
}