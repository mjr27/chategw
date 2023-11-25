using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Application.Messaging.Archive;

/// <inheritdoc />
public class DeleteEntryFromArchiveHandler : ApplicationCommandHandler<DeleteEntryFromArchiveInput>
{
    /// <inheritdoc />
    public override async Task Handle(DeleteEntryFromArchiveInput request, CancellationToken cancellationToken)
    {
        int updatedCount = await this._db.PublicationArchive
            .Where(r => r.PublicationId == request.PublicationId && r.Id == request.Id)
            .ExecuteUpdateAsync(r => r.SetProperty(p => p.DeletedAt, Now), cancellationToken);

        if (updatedCount == 0)
        {
            throw new NotFoundProblemDetailsException("Archived entity not found");
        }
    }

    /// <inheritdoc />
    public DeleteEntryFromArchiveHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}