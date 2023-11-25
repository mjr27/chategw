using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Messaging.Publications;

/// <inheritdoc />
public class SetPublicationMetadataHandler : ApplicationCommandHandler<SetPublicationMetadataInput>
{

    /// <inheritdoc />
    public override async Task Handle(SetPublicationMetadataInput request, CancellationToken cancellationToken)
    {
        Publication publication = await _db.Publications
                                      .FirstOrDefaultAsync(r => r.PublicationId == request.PublicationId, cancellationToken)
                                  ?? throw new NotFoundProblemDetailsException("Publication not found");

        publication.ChangeMetadata(
            request.Code,
            request.Title,
            request.Description,
            request.AuthorId,
            request.Publisher,
            request.PageCount,
            request.PublicationYear,
            request.Isbn,
            Now
        );
        await _db.SaveChangesAsync(cancellationToken); 
    }

    /// <inheritdoc />
    public SetPublicationMetadataHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}