using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

using Dapper;

using Egw.PubManagement.Persistence.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Egw.ImportUtils.Services.Importers;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local")]
public class ExportImporterService : ImporterServiceBase
{
    private readonly ILogger<ExportImporterService> _logger;
    private readonly Uri _baseMediaUri;
    private readonly FileInfo _outputFile;

    public ExportImporterService(
        ILogger<ExportImporterService> logger,
        Uri baseMediaUri,
        FileInfo outputFile,
        string publicationDbConnectionString,
        string egwConnectionString
    ) : base(publicationDbConnectionString, egwConnectionString)
    {
        _logger = logger;
        _baseMediaUri = baseMediaUri;
        _outputFile = outputFile;
    }

    private record InnerPublicationDto(int PublicationId, string Code, string LanguageCode, string? OriginalCode);

    private record InnerDownloadedFileDto(Guid Id, int PublicationId,
        [TypeConverter(typeof(EnumConverter), typeof(ExportTypeEnum))]
        ExportTypeEnum Type, Uri Uri);

    public override async Task Import(CancellationToken cancellationToken)
    {
        List<InnerPublicationDto> publications = await _db.Publications
            .Include(r => r.Placement)
            .Include(r => r.Language)
            .Where(r => r.Placement != null)
            .Select(r => new InnerPublicationDto(
                r.PublicationId,
                r.Code,
                r.Language.EgwCode,
                r.OriginalPublicationId == null
                    ? null
                    : r.OriginalPublication!.Code
            )).ToListAsync(cancellationToken);

        var availablePublications = publications
            .Select(r => r.PublicationId)
            .ToList();
        var files = _egwDb.Query(FilesQuery, new { publications = availablePublications })
            .ToList();
        var swTotal = Stopwatch.StartNew();
        var exportedFiles = new List<InnerDownloadedFileDto>();

        IEnumerable<IGrouping<string, dynamic>> groupedPublicationTypes = files.GroupBy(r => (string)r.pubtype);
        foreach (IGrouping<string, dynamic> group in groupedPublicationTypes)
        {
            string pubType = group.Key;
            ExportTypeEnum exportType = pubType switch
            {
                "epub" => ExportTypeEnum.Epub,
                "mobi" => ExportTypeEnum.Mobi,
                "mp3" => ExportTypeEnum.Mp3,
                "pdf" => ExportTypeEnum.Pdf,
                _ => throw new InvalidOperationException($"Invalid export type : {pubType}")
            };
            var typePublications = group.Select(r => (int)r.pubnr).ToList();
            var filteredPublications = publications.Where(r => typePublications.Contains(r.PublicationId)).ToList();
            exportedFiles.AddRange(GetExportedFiles(exportType, filteredPublications));
        }

        _db.ChangeTracker.Clear();
        await _db.PublicationExports.ExecuteDeleteAsync(cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);
        await using var f = new StreamWriter(_outputFile.FullName,
            options: new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.Create });
        await using var writer = new CsvWriter(f, config);
        await writer.WriteRecordsAsync(exportedFiles, cancellationToken);
        _logger.LogError("{Count} publications were processed in {Elapsed}",
            exportedFiles.DistinctBy(r => r.PublicationId).Count(), swTotal.Elapsed);
    }

    private IEnumerable<InnerDownloadedFileDto> GetExportedFiles(ExportTypeEnum type,
        List<InnerPublicationDto> publications)
    {
        return publications
            .Select(publication => new InnerDownloadedFileDto(
                    Guid.NewGuid(),
                    publication.PublicationId,
                    type,
                    new Uri(
                        _baseMediaUri,
                        GetFolder(type) + '/' +
                        GetFileName(
                            publication.PublicationId,
                            type,
                            publication.LanguageCode,
                            publication.Code,
                            publication.OriginalCode
                        )
                    )
                )
            );
    }

    private const string FilesQuery = @"-- noinspection SqlDialectInspectionForFile
SELECT pubnr, substr(`key`, 6) as pubtype
FROM sqlb_publicationoverviewadd
WHERE `key` IN ('file_mp3', 'file_pdf', 'file_epub', 'file_mobi')
    AND pubnr in @publications
";

    private static string GetFileName(
        int publicationId,
        ExportTypeEnum type,
        string language,
        string code,
        string? codeEn)
    {
        string fileType = type switch
        {
            ExportTypeEnum.Epub => "epub",
            ExportTypeEnum.Mobi => "mobi",
            ExportTypeEnum.Mp3 => "mp3",
            ExportTypeEnum.Pdf => "pdf",
            _ => throw new ArgumentException($"Invalid export type : {type}")
        };
        string filename = language != "en" && !string.IsNullOrWhiteSpace(codeEn)
            ? $"{code}({codeEn})"
            : code;

        return type == ExportTypeEnum.Mp3
            ? $"{publicationId}/{language}_{filename}.zip"
            : $"{language}_{filename}.{fileType}";
    }

    private static string GetFolder(ExportTypeEnum type) => type switch
    {
        ExportTypeEnum.Epub => "epub",
        ExportTypeEnum.Mobi => "mobi",
        ExportTypeEnum.Mp3 => "mp3",
        ExportTypeEnum.Pdf => "pdf",
        _ => throw new ArgumentException($"Invalid export type : {type}")
    };
}