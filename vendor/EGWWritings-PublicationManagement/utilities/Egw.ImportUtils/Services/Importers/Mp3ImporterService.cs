using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using Dapper;

using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using WhiteEstate.DocFormat;
namespace Egw.ImportUtils.Services.Importers;

public class Mp3ImporterService : ImporterServiceBase
{
    private readonly ILogger<Mp3ImporterService> _logger;
    private readonly string _mp3DataUri;

    public Mp3ImporterService(
        ILogger<Mp3ImporterService> logger,
        string publicationDbConnectionString,
        string egwConnectionString,
        string mp3DataUri
    ) : base(publicationDbConnectionString, egwConnectionString)
    {
        _logger = logger;
        _mp3DataUri = mp3DataUri;
    }

    private class Mp3FileRecord
    {
        [JsonPropertyName("file")]
        public required string File { get; init; }
        [JsonPropertyName("book_id")]
        public required int BookId { get; init; }
        [JsonPropertyName("size")]
        public required long Size { get; init; }
        [JsonPropertyName("duration")]
        public required long Duration { get; init; }
    }

    public override async Task Import(CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        List<Mp3FileRecord> mp3List = await LoadMp3FileList(cancellationToken);
        var availablePublications = _db.PublicationPlacement
            .AsNoTracking()
            .Select(r => r.PublicationId)
            .ToList();
        var publications = _egwDb.Query<int>(Mp3PublicationsQuery, new { publications = availablePublications })
            .ToList();
        var swTotal = Stopwatch.StartNew();
        var mp3FileList = new List<Mp3File>();
        _logger.LogInformation("Got mp3 files: {Count}", mp3List.Count);
        var groupedMp3Files = mp3List
            .ToDictionary(r => (r.BookId, r.File), r => (r.Size, r.Duration));
        _logger.LogInformation("Grouping done");
        foreach (int publicationId in publications)
        {
            IEnumerable<string> fileDetails = _egwDb.Query<string>(Mp3FilesQuery, new { publicationId });
            foreach (string? fileName in fileDetails)
            {
                string baseName = Path.GetFileNameWithoutExtension(fileName);
                string[] chunks = baseName.Split('_');
                VoiceTypeEnum voice = chunks[2] switch
                {
                    "m" => VoiceTypeEnum.Male,
                    "f" => VoiceTypeEnum.Female,
                    _ => throw new ArgumentException($"Invalid voice type : {chunks[2]}")
                };
                int elementId = int.Parse(chunks[^1]);
                if (!groupedMp3Files.TryGetValue((publicationId, fileName), out (long Size, long Duration) file))
                {
                    _logger.LogWarning("Cannot find file {Filename} in {Publication}", fileName, publicationId);
                    continue;
                }

                mp3FileList.Add(new Mp3File(new ParaId(publicationId, elementId), voice, fileName, file.Size, file.Duration, now));
            }
        }

        _db.ChangeTracker.Clear();
        await _db.PublicationMp3Files.ExecuteDeleteAsync(cancellationToken);
        _db.PublicationMp3Files.AddRange(mp3FileList);
        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogError("{Count} publications were processed in {Elapsed}", publications.Count, swTotal.Elapsed);
    }

    private async Task<List<Mp3FileRecord>> LoadMp3FileList(CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        string response = await client.GetStringAsync(_mp3DataUri, cancellationToken);
        return JsonSerializer.Deserialize<List<Mp3FileRecord>>(response) ?? throw new JsonException("Unable to parse mp3 data");
    }

    private const string Mp3PublicationsQuery = @"-- noinspection SqlDialectInspectionForFile
select distinct a.pubnr
from sqlb_publicationoverviewadd a
where `key` = 'mp3startid'
  and a.data2 != ''
  and a.pubnr in @publications
";
    private const string Mp3FilesQuery = @"-- noinspection SqlDialectInspectionForFile
    select a.data2       as Filename 
    from sqlb_publicationoverviewadd a
             inner join sqlb_publicationoverview po on po.pubnr = a.pubnr
    where po.actversion = 1
      and a.pubnr = @publicationId
      and `key` = 'mp3startid'
      and a.data2 != '';
    ";
}