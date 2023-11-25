using Egw.PubManagement.Application.GraphQl.Loaders;
using Egw.PubManagement.Application.Models.Internal;
using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Messaging.Mp3;

/// <inheritdoc />
public class UpdateMp3FilesHandler : ApplicationCommandHandler<UpdateMp3FilesInput>
{
    private readonly Mp3ManifestService _manifestService;
    private readonly CloudMp3FilesLoader _loader;
    private List<Mp3ManifestType> _manifest = new();

    /// <inheritdoc />
    public UpdateMp3FilesHandler(IStorageWrapper storageWrapper, Mp3ManifestService manifestService,
        PublicationDbContext db, IClock clock) : base(db, clock)
    {
        _manifestService = manifestService;
        _loader = new CloudMp3FilesLoader(storageWrapper);
    }

    /// <inheritdoc />
    public async override Task Handle(UpdateMp3FilesInput request, CancellationToken cancellationToken)
    {
        _manifest = await _manifestService.LoadManifest(request.PublicationId, cancellationToken);
        await Validate(request, cancellationToken);
        await Save(request, cancellationToken);
    }

    private async Task<bool> Save(UpdateMp3FilesInput data, CancellationToken cancellationToken)
    {
        IDbContextTransaction trans = await _db.Database.BeginTransactionAsync(cancellationToken);
        _db.PublicationMp3Files.RemoveRange(_db.PublicationMp3Files.Where(r => r.PublicationId == data.PublicationId));
        _db.PublicationMp3Files.AddRange(data.Mp3Files.Select(CreateMp3Item));
        await _db.SaveChangesAsync(cancellationToken);
        await trans.CommitAsync(cancellationToken);
        return true;
    }

    // ReSharper disable once CognitiveComplexity
    private async Task Validate(UpdateMp3FilesInput request, CancellationToken cancellationToken)
    {
        var errorsList = new List<ValidationFailure>();

        // Chapter ParaId != ParaId in file name
        foreach (AssignedMp3File row in request.Mp3Files)
        {
            if (row.File.ParaId != row.ParaId)
            {
                errorsList.Add(new ValidationFailure
                {
                    ErrorMessage =
                        $"File ParaId - {row.File.ParaId} must be equal Chapter ParaId - {row.ParaId}",
                    PropertyName = "mp3Files.ParaId"
                });
            }
        }

        var duplicateChapter = request.Mp3Files
            .GroupBy(r => r.File.FileName)
            .Where(r => r.Count() > 1)
            .ToDictionary(r => r.Key, r => r.Select(f => f.ParaId));
        if (duplicateChapter.Any())
        {
            errorsList.AddRange(duplicateChapter.Select(r => new ValidationFailure
            {
                ErrorMessage =
                    $"File: {r.Key} Assign to several chapters: ({string.Join(", ", r.Value.Select(p => p.ToString()))})",
                PropertyName = "mp3Files.ParaId"
            }));
        }

        var duplicateFile = request.Mp3Files
            .GroupBy(r => r.ParaId)
            .Where(r => r.Count() > 1)
            .ToDictionary(r => r.Key, r => r.Select(f => f.File).ToList());

        // multiple mp3 files with different voice
        foreach ((ParaId chapterPara, List<Mp3FileName> files) in duplicateFile)
        {
            if (files.Count > 2)
            {
                continue;
            }

            if (files[0].Number == files[1].Number &&
                files[0].Lang == files[1].Lang &&
                files[0].Voice != files[1].Voice)
            {
                duplicateFile.Remove(chapterPara);
            }
        }

        if (duplicateFile.Any())
        {
            errorsList.AddRange(duplicateFile.Select(r => new ValidationFailure
            {
                ErrorMessage = $"Chapter: {r.Key} has multiple files Assigned: ({string.Join(", ", r.Value)})",
                PropertyName = "mp3Files.ParaId"
            }));
        }

        List<Mp3FileItem> filesFromCloud = await _loader.Get(request.PublicationId, cancellationToken);

        var filesName = filesFromCloud.Select(r => r.FileName).ToList();
        var chapters = _db.PublicationChapters
            .AsNoTracking()
            .Where(r => r.PublicationId == request.PublicationId)
            .OrderBy(r => r.Order)
            .Select(r => r.ParaId)
            .ToList();
        var chapterNotFound = request.Mp3Files
            .Where(r => !chapters.Contains(r.ParaId))
            .Select(r => r.ParaId)
            .Distinct()
            .ToList();

        if (chapterNotFound.Any())
        {
            errorsList.AddRange(chapterNotFound.Select((r, i) => new ValidationFailure
            {
                ErrorMessage = $"Chapter not found in DB: {r.ToString()}", PropertyName = $"mp3Files[{i}].ParaId"
            }));
        }

        var filesNotFoundInManifest = request.Mp3Files
            .Where(r => !_manifest.Select(m => m.File.FileName).Contains(r.File.FileName))
            .Select(r => r.File)
            .ToList();

        if (filesNotFoundInManifest.Any())
        {
            errorsList.AddRange(filesNotFoundInManifest.Select((r, i) => new ValidationFailure
            {
                ErrorMessage = $"File not found in manifest (update it): {r.ToString()}",
                PropertyName = $"mp3Files[{i}].FileName"
            }));
        }

        var filesNotFound = request.Mp3Files
            .Where(r => !filesName.Contains(r.File.FileName))
            .Select(r => r.File)
            .ToList();

        if (filesNotFound.Any())
        {
            errorsList.AddRange(filesNotFound.Select((r, i) => new ValidationFailure
            {
                ErrorMessage = $"File not found on cloud: {r.ToString()}", PropertyName = $"mp3Files[{i}].FileName"
            }));
        }

        if (errorsList.Any())
        {
            throw new ValidationException(errorsList);
        }
    }

    private Mp3File CreateMp3Item(AssignedMp3File f)
    {
        Mp3ManifestType? mp3Info = _manifest.FirstOrDefault(r => r.File == f.File);
        if (mp3Info is null)
        {
            throw new ValidationException($"{f.File} not found in manifest");
        }

        return new Mp3File(f.ParaId,
            f.File.Voice,
            f.File.FileName,
            mp3Info.Size,
            mp3Info.Duration,
            DateTimeOffset.UtcNow);
    }
}