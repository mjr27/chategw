using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using Microsoft.EntityFrameworkCore;

using SixLabors.ImageSharp;
namespace Egw.PubManagement.Application.Messaging.Covers;

/// <inheritdoc />
public class UploadCoverHandler : ApplicationCommandHandler<UploadCoverInput>
{
    private readonly IStorageWrapper _storageWrapper;

    /// <inheritdoc />
    public override async Task Handle(UploadCoverInput request, CancellationToken cancellationToken)
    {
        Guid id = request.Id ?? throw new ValidationProblemDetailsException("Id is required");
        CoverType? type = await _db.CoverTypes.FirstOrDefaultAsync(r => r.Code == request.Type, cancellationToken);
        if (type is null)
        {
            throw new ValidationProblemDetailsException("Type does not exist");
        }

        if (!await _db.Publications.AnyAsync(r => r.PublicationId == request.PublicationId, cancellationToken))
        {
            throw new ValidationProblemDetailsException("Publication does not exist");
        }

        if (await _db.Covers.AnyAsync(r => r.Id == request.Id, cancellationToken))
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

            ImageInfo imageInfo = await Image.IdentifyAsync(temporaryFile, cancellationToken);
            if (imageInfo.Metadata.DecodedImageFormat is null)
            {
                throw new ValidationProblemDetailsException("Invalid image format. Only jpeg and png are allowed");
            }

            MediaFormatEnum format = imageInfo.Metadata.DecodedImageFormat.Name switch
            {
                "JPEG" => MediaFormatEnum.Jpeg,
                "PNG" => MediaFormatEnum.Png,
                _ => throw new ValidationProblemDetailsException("Invalid image format. Only jpeg and png are allowed")
            };

            if (imageInfo.Width < type.MinWidth)
            {
                throw new ValidationProblemDetailsException($"Image width must be at least {type.MinWidth}");
            }

            float proportion = (float)imageInfo.Width / imageInfo.Height;
            float originalProportion = (float)type.MinWidth / type.MinHeight;
            if (Math.Abs(proportion - originalProportion) > 0.01)
            {
                throw new ValidationProblemDetailsException($"Image proportions must be {type.MinWidth}:{type.MinHeight} +/- 1%");
            }

            string idChunk = (request.PublicationId / 100 * 100).ToString("00000");
            string extension = imageInfo.Metadata.DecodedImageFormat.FileExtensions.First();
            string fileName = $"{idChunk}/{request.PublicationId}/{id:N}.{extension}";
            var cover = new Cover
            {
                Id = id,
                PublicationId = request.PublicationId,
                TypeId = request.Type,
                Size = fileInfo.Length,
                Format = format,
                Height = imageInfo.Height,
                Width = imageInfo.Width,
                Uri = new Uri(fileName, UriKind.Relative),
                CreatedAt = Now,
                UpdatedAt = Now,
                IsMain = false
            };
            _db.Covers.Add(cover);
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
    public UploadCoverHandler(PublicationDbContext db, IClock clock, IStorageWrapper storageWrapper) : base(db, clock)
    {
        _storageWrapper = storageWrapper;
    }
}