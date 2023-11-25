using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Covers;

/// <inheritdoc />
public class CreateCoverTypeHandler : ApplicationCommandHandler<CreateCoverTypeInput>
{
    /// <inheritdoc />
    public override async Task Handle(CreateCoverTypeInput request, CancellationToken cancellationToken)
    {
        bool sameCodeExists = await _db.CoverTypes.AnyAsync(ct => ct.Code == request.Code, cancellationToken);
        if (sameCodeExists)
        {
            throw new ConflictProblemDetailsException("Cover type with same code already exists");
        }

        var coverType = new CoverType
        {
            Code = request.Code, Description = request.Description, MinWidth = request.MinWidth, MinHeight = request.MinHeight
        };
        _db.CoverTypes.Add(coverType);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public CreateCoverTypeHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}