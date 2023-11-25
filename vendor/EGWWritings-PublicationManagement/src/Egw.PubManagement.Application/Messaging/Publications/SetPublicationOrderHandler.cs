using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Publications;

/// <inheritdoc />
public class SetPublicationOrderHandler : ApplicationCommandHandler<SetPublicationOrderInput>
{

    /// <inheritdoc />
    public override async Task Handle(SetPublicationOrderInput request, CancellationToken cancellationToken)
    {
        var placementInfo = await _db.PublicationPlacement
            .Where(r => r.PublicationId == request.PublicationId)
            .Select(r => new { r.FolderId })
            .FirstOrDefaultAsync(cancellationToken);
        if (placementInfo is null)
        {
            throw new NotFoundProblemDetailsException("Publication not found");
        }

        PublicationPlacement[] placements = await _db.PublicationPlacement
            .Where(r => r.FolderId == placementInfo.FolderId)
            .OrderBy(r => r.Order)
            .ToArrayAsync(cancellationToken);

        foreach (PublicationPlacement row in placements)
        {
            int newOrder = row.PublicationId == request.PublicationId
                ? (int)request.Order
                : row.Order > request.Order
                    ? row.Order + 1
                    : row.Order - 1;

            row.ChangePlacement(row.FolderId, newOrder + 1000, Now);
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _db.FixFolderOrdering(placementInfo.FolderId, Now, cancellationToken);
    }

    /// <inheritdoc />
    public SetPublicationOrderHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }


}