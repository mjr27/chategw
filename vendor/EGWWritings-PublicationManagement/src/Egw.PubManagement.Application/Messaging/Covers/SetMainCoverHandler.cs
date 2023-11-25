using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Covers;

/// <inheritdoc />
public class SetMainCoverHandler : ApplicationCommandHandler<SetMainCoverInput>
{
    /// <inheritdoc />
    public override async Task Handle(SetMainCoverInput request, CancellationToken cancellationToken)
    {
        await _db.Covers.Where(r => r.PublicationId == request.PublicationId && r.TypeId == request.Type && r.IsMain)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(b => b.IsMain, false)
                    .SetProperty(b => b.UpdatedAt, Now),
                cancellationToken);


        Cover? cover = await _db.Covers
            .Where(r => r.PublicationId == request.PublicationId && r.TypeId == request.Type && r.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (cover is not null)
        {
            cover.IsMain = true;
            cover.UpdatedAt = Now;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public SetMainCoverHandler(PublicationDbContext db, IClock clock) :
        base(db, clock)
    {
    }
}