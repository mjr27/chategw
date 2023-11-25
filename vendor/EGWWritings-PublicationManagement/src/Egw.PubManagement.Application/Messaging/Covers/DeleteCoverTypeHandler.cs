using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Covers;

/// <inheritdoc />
public class DeleteCoverTypeHandler : ApplicationCommandHandler<DeleteCoverTypeInput>
{

    /// <inheritdoc />
    public override async Task Handle(DeleteCoverTypeInput request, CancellationToken cancellationToken)
    {
        CoverType type = await _db.CoverTypes.FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken)
                         ?? throw new NotFoundProblemDetailsException("Cover type not found");
        bool hasChildren = await _db.Covers.AnyAsync(r => r.TypeId == request.Code, cancellationToken);
        if (hasChildren)
        {
            throw new ConflictProblemDetailsException("Cover type has children");
        }

        _db.CoverTypes.Remove(type);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public DeleteCoverTypeHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}