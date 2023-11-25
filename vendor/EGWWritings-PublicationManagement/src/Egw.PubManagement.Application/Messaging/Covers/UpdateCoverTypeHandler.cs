using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Covers;

/// <inheritdoc />
public class UpdateCoverTypeHandler : ApplicationCommandHandler<UpdateCoverTypeInput>
{

    /// <inheritdoc />
    public override async Task Handle(UpdateCoverTypeInput request, CancellationToken cancellationToken)
    {
        CoverType type = await _db.CoverTypes.FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken)
                         ?? throw new NotFoundProblemDetailsException("Cover type not found");
        type.Description = request.Description;
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public UpdateCoverTypeHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }

}