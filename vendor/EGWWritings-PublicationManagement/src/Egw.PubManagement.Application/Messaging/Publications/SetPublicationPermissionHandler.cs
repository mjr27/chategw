using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Publications;

/// <inheritdoc />
public class SetPublicationPermissionHandler : ApplicationCommandHandler<SetPublicationPermissionInput>
{
    /// <inheritdoc />
    public SetPublicationPermissionHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }

    /// <inheritdoc />
    public override async Task Handle(
        SetPublicationPermissionInput request,
        CancellationToken cancellationToken
    )
    {
        PublicationPlacement? placement = await _db.PublicationPlacement
            .Include(r => r.Publication)
            .FirstOrDefaultAsync(r => r.PublicationId == request.PublicationId, cancellationToken);
        if (placement is null)
        {
            throw new NotFoundProblemDetailsException("Publication placement not found");
        }

        placement.ChangePermission(request.Permission, Now);
        await _db.SaveChangesAsync(cancellationToken);
    }
}