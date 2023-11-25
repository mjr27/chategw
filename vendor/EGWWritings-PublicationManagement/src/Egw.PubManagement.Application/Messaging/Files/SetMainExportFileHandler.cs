using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Application.Messaging.Files;

/// <summary> Handler for <see cref="SetMainExportFileInput"/> </summary>
public class SetMainExportFileHandler : ApplicationCommandHandler<SetMainExportFileInput>
{
    /// <inheritdoc />
    public override async Task Handle(SetMainExportFileInput request, CancellationToken cancellationToken)
    {
        ExportedFile? existingFile = await _db.PublicationExports
            .Where(r => r.PublicationId == request.PublicationId && r.Type == request.Type && r.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (existingFile is null)
        {
            throw new NotFoundProblemDetailsException($"Exported file {request.Id} not found");
        }

        if (existingFile.IsMain)
        {
            return;
        }

        await _db.PublicationExports
            .Where(r => r.PublicationId == request.PublicationId && r.Type == request.Type)
            .ExecuteUpdateAsync(r => r.SetProperty(p => p.IsMain, false).SetProperty(p => p.UpdatedAt, Now),
                cancellationToken);
        existingFile.UpdatedAt = Now;
        existingFile.IsMain = true;
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public SetMainExportFileHandler(PublicationDbContext db, IClock clock) : base(db, clock)
    {
    }
}