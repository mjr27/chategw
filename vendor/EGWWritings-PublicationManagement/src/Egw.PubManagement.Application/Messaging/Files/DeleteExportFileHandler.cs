using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Storage;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Application.Messaging.Files;

/// <summary> Handler for <see cref="DeleteExportFileInput"/> </summary>
public class DeleteExportFileHandler : ApplicationCommandHandler<DeleteExportFileInput>
{
    private readonly IStorage _storage;

    /// <inheritdoc />
    public override async Task Handle(DeleteExportFileInput request, CancellationToken cancellationToken)
    {
        ExportedFile? file = await _db.PublicationExports
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (file is null)
        {
            throw new NotFoundProblemDetailsException($"Exported file {request.Id} not found");
        }

        if (file.IsMain)
        {
            throw new ConflictProblemDetailsException("Cannot delete main file");
        }

        _db.PublicationExports.Remove(file);
        await _db.SaveChangesAsync(cancellationToken);
        await _storage.Delete(file.Uri.ToString(), cancellationToken);
    }

    /// <inheritdoc />
    public DeleteExportFileHandler(PublicationDbContext db, IClock clock, IStorageWrapper storageWrapper) : base(db,
        clock)
    {
        _storage = storageWrapper.Exports;
    }
}