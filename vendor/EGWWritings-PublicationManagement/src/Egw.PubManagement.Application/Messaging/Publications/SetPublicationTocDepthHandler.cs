using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Messaging.Publications;

/// <inheritdoc />
public class SetPublicationTocDepthHandler : ApplicationCommandHandler<SetPublicationTocDepthInput>
{
    /// <inheritdoc />
    public SetPublicationTocDepthHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }

    /// <inheritdoc />
    public override async Task Handle(
        SetPublicationTocDepthInput request,
        CancellationToken cancellationToken
    )
    {
        PublicationPlacement placement = await _db.PublicationPlacement
                      .FirstOrDefaultAsync(r => r.PublicationId == request.PublicationId, cancellationToken)
                      ?? throw new NotFoundProblemDetailsException("Publication placement not found");

        int? tocDepth = request.TocDepth;

        //if needed toc depth calculation
        if (tocDepth is null) 
        {
            Publication publication = await _db.Publications
                      .FirstOrDefaultAsync(r => r.PublicationId == request.PublicationId, cancellationToken)
                      ?? throw new NotFoundProblemDetailsException("Publication not found");

            tocDepth = DbExtensions.GetDefaultHeadingLevel(publication.Type);

            List<ParaId> mp3Files = await _db.PublicationMp3Files
                .Where(r => r.PublicationId == request.PublicationId)
                .Select(r => r.ParaId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var headings = await _db.Paragraphs
                .Where(r => r.PublicationId == request.PublicationId)
                .Where(p => mp3Files.Contains(p.ParaId))
                .Select(r => new { r.ParaId, r.HeadingLevel })
                .ToListAsync(cancellationToken);

            if (mp3Files.Count > 0 && headings.Count == mp3Files.Count && !headings.Any(r => r.HeadingLevel is null or 0))
            {
                tocDepth = headings
                    .Where(r => r.HeadingLevel is not (0 or null))
                    .Select(r => r.HeadingLevel!.Value)
                    .DefaultIfEmpty()
                    .Max();
            }
        }

        placement.ChangeTocDepth(tocDepth, Now);
        await _db.SaveChangesAsync(cancellationToken);
    }
}