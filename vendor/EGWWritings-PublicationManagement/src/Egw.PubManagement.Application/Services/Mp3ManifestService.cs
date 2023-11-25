using System.Text.Json;
using System.Text.RegularExpressions;

using ATL;

using Egw.PubManagement.Application.Messaging.Mp3;
using Egw.PubManagement.Application.Models.Internal;
using Egw.PubManagement.Storage;

using Microsoft.Extensions.Logging;

namespace Egw.PubManagement.Application.Services;

/// <summary>
/// Mp3 Manifest service
/// </summary>
public class Mp3ManifestService
{
    private readonly IStorageWrapper _storage;
    private readonly ILogger<Mp3ManifestService> _logger;
    private const int ThreadCount = 6;

    /// <summary>
    /// Manifest file name (one in each folder)
    /// </summary>
    public const string ManifestFileName = "manifest.json";

    // private Regex _reMp3FileTemplate = new(".*");

    /// <summary>
    /// Create | update manifest for mp3 files
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="logger"></param>
    public Mp3ManifestService(IStorageWrapper storage, ILogger<Mp3ManifestService> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    /// <summary>
    /// Update manifest
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    public async Task UpdateManifest(UpdateMp3ManifestInput input, CancellationToken cancellationToken)
    {
        var manifestMp3Files = new List<Mp3ManifestType>();
        var reMp3FileTemplate = new Regex(
            @$"^{input.PublicationId}\/(?<fileNumber>[0-9]{{4}})_(?<lang>[a-z]{{3}})_(?<voice>[a-z])_(?<name>[a-z0-9_]+)_(?<publicationId>[0-9]+)_(?<paragraphId>[0-9]+)\.mp3$",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        var files = new List<IStorageBlob>();
        Console.WriteLine($"Processing {input.PublicationId}");
        await foreach (IStorageBlob r in _storage.Mp3.ListObjects($"{input.PublicationId}", cancellationToken))
        {
            files.Add(r);
        }

        IStorageBlob? manifestFileInfo = files.FirstOrDefault(r => r.FileName == ManifestFileName);

        files = files.Where(r => reMp3FileTemplate.IsMatch(r.Path)).ToList();

        var manifest = new List<Mp3ManifestType>();
        if (manifestFileInfo is not null)
        {
            manifest = await LoadManifest(input.PublicationId, cancellationToken);
        }

        DateTime lastModify = manifestFileInfo?.LastModified ?? DateTime.MinValue;

        List<IStorageBlob> filesForUpdate;

        if (input.ReplaceManifest)
        {
            filesForUpdate = files;
        }
        else
        {
            filesForUpdate = files
                .Where(r => r.LastModified > lastModify
                            || !manifest.Select(m => m.File.FileName).Contains(r.FileName))
                .ToList();

            manifestMp3Files.AddRange(manifest
                .Where(r => !filesForUpdate.Select(f => f.FileName).Contains(r.File.FileName)
                            && files.Select(f => f.FileName).Contains(r.File.FileName))
            );
        }

        if (!filesForUpdate.Any())
        {
            _logger.LogInformation("All mp3 files up to date");
            return;
        }

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = ThreadCount };

        await Parallel.ForEachAsync(filesForUpdate, parallelOptions, async (file, ct) =>
        {
            try
            {
                await GetMp3FileInfo(file, manifestMp3Files, ct);
            }
            catch (Exception e)
            {
                _logger.LogError("Problem while getting info for {FileName}: {Message}", file.Path, e.Message);
                throw;
            }
        });

        try
        {
            await _storage.Mp3.Write(
                $"{input.PublicationId}/{ManifestFileName}",
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(manifestMp3Files.OrderBy(r => r.File.Number))),
                cancellationToken);
            _logger.LogInformation("Manifest for #{PubId}# - uploaded", input.PublicationId);
        }
        catch (Exception e)
        {
            _logger.LogError("Problem while uploading manifest.json: {Message}", e.Message);
            throw;
        }
    }

    /// <summary>
    /// Load data manifest from cloud
    /// </summary>
    /// <param name="publicationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<Mp3ManifestType>> LoadManifest(int publicationId, CancellationToken cancellationToken)
    {
        string path = $"{publicationId}/{ManifestFileName}";
        Stream? stream = await _storage.Mp3.Read(path, cancellationToken);
        if (stream is null)
        {
            return new List<Mp3ManifestType>();
        }

        using var reader = new StreamReader(stream);
        string content = await reader.ReadToEndAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<Mp3ManifestType>>(content) ?? new List<Mp3ManifestType>();
    }

    private async Task GetMp3FileInfo(IStorageBlob file, ICollection<Mp3ManifestType> manifestMp3Files,
        CancellationToken cancellationToken)
    {
        await using Stream? stream = await _storage.Mp3.Read(file.Path, cancellationToken);
        using var memStream = new MemoryStream();
        if (stream is null)
        {
            _logger.LogInformation("Stream for {FileName} is null", file.Path);
            return;
        }

        await stream.CopyToAsync(memStream, cancellationToken);
        var track = new Track(memStream);

        if (Mp3FileName.TryParse(file.Path, out Mp3FileName mp3))
        {
            manifestMp3Files.Add(new Mp3ManifestType
            {
                Duration = track.Duration,
                File = mp3,
                Size = file.Size,
                ParaId = mp3.ParaId,
                Voice = mp3.Voice,
                BookId = mp3.ParaId.PublicationId,
                ElementId = mp3.ParaId.ElementId
            });
            _logger.LogInformation("{FileName} - parsed", file.Path);
        }
        else
        {
            _logger.LogInformation("{FileName} - incorrect filename template", file.Path);
        }
    }
}