using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Publications;

/// <summary>
/// Unpublishes a publication
/// </summary>
public class UnpublishPublicationHandler : ApplicationCommandHandler<UnpublishPublicationInput>
{
    /// <inheritdoc />
    public override async Task Handle(UnpublishPublicationInput request, CancellationToken cancellationToken)
    {
        PublicationPlacement? placement = await _db.PublicationPlacement
            .Where(r => r.PublicationId == request.PublicationId)
            .FirstOrDefaultAsync(cancellationToken);
        if (placement is null)
        {
            throw new NotFoundProblemDetailsException("Publication not found");
        }

        int folderId = placement.FolderId;
        _db.PublicationPlacement.Remove(placement);
        await _db.SaveChangesAsync(cancellationToken);
        await _db.FixFolderOrdering(folderId, Now, cancellationToken);
    }

    /// <inheritdoc />
    public UnpublishPublicationHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }


}