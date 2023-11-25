using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Application.Messaging.Files;

/// <inheritdoc />
public class UploadExportHandler : ApplicationCommandHandler<UploadExportInput>
{
    private readonly IStorageWrapper _storageWrapper;

    /// <inheritdoc />
    public override async Task Handle(UploadExportInput request, CancellationToken cancellationToken)
    {
        if (!await _db.Publications.AnyAsync(r => r.PublicationId == request.PublicationId, cancellationToken))
        {
            throw new ValidationProblemDetailsException("Publication does not exist");
        }

        if (await _db.PublicationExports.AnyAsync(r => r.Id == request.Id, cancellationToken))
        {
            throw new ConflictProblemDetailsException("Cover already exists");
        }

        string temporaryFile = Path.GetTempFileName();
        await using (FileStream f = File.Open(temporaryFile, FileMode.Create))
        {
            await request.File.CopyToAsync(f, cancellationToken);
        }

        try
        {
            var fileInfo = new FileInfo(temporaryFile);

            string idChunk = (request.PublicationId / 100 * 100).ToString("00000");
            string extension = request.Type switch
            {
                ExportTypeEnum.Epub => "epub",
                ExportTypeEnum.Mobi => "mobi",
                ExportTypeEnum.Mp3 => "zip",
                ExportTypeEnum.Pdf => "pdf",
                _ => throw new InvalidOperationException($"Invalid export type : {request.Type}")
            };
            string fileName = $"{idChunk}/{request.PublicationId}/{request.Id:N}.{extension}";
            var cover = new ExportedFile(request.Id, request.PublicationId, Now)
            {
                Size = fileInfo.Length, Type = request.Type, IsMain = false, Uri = new Uri(fileName, UriKind.Relative),
            };
            _db.PublicationExports.Add(cover);
            await _db.SaveChangesAsync(cancellationToken);
            await using FileStream f2 = File.OpenRead(temporaryFile);
            await _storageWrapper.Covers.Write(fileName, f2, cancellationToken);
        }
        finally
        {
            File.Delete(temporaryFile);
        }
    }

    /// <inheritdoc />
    public UploadExportHandler(PublicationDbContext db, IClock clock, IStorageWrapper storageWrapper) : base(db, clock)
    {
        _storageWrapper = storageWrapper;
    }
}