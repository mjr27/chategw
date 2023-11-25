using System.Text.RegularExpressions;

using Egw.PubManagement.Application.Models.Internal;
using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Persistence.Enums;
using Egw.PubManagement.Storage;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.GraphQl.Loaders;

/// <summary>
/// 
/// </summary>
public class CloudMp3FilesLoader
{
    private readonly IStorageWrapper _storageWrapper;

    /// <summary>
    /// Get mp3 files information from AWS and Db
    /// </summary>
    /// <param name="storageWrapper"></param>
    public CloudMp3FilesLoader(IStorageWrapper storageWrapper)
    {
        _storageWrapper = storageWrapper;
    }

    /// <summary>
    /// Get mp3 files list
    /// </summary>
    /// <param name="publicationId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<List<Mp3FileItem>> Get(int publicationId, CancellationToken ct)
    {
        var reMp3FileTemplate = new Regex(
            @$"^{publicationId}\/(?<fileNumber>[0-9]{{4}})_(?<lang>[a-z]{{3}})_(?<voice>[a-z])_(?<name>[a-z0-9_]+)_(?<publicationId>[0-9]+)_(?<paragraphId>[0-9]+)\.mp3$",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        var result = new List<Mp3FileItem>();
        await foreach (IStorageBlob file in _storageWrapper.Mp3.ListObjects($"{publicationId}", ct))
        {
            Match m = reMp3FileTemplate.Match(file.Path);
            if (m.Success && Mp3FileName.TryParse(file.Path, out Mp3FileName item))
            {
                result.Add(new Mp3FileItem
                {
                    FileName = item.FileName,
                    ParsedData = new Mp3ParsedData
                    {
                        ParaId = item.ParaId, FileNumber = item.Number, Voice = item.Voice, Path = item.Path
                    }
                });
            }
        }

        return result;
    }
}

/// <summary>
/// List of mp3 files from cloud
/// </summary>
public class Mp3FileItem
{
    /// <summary>
    /// File name
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Information from filename
    /// </summary>
    public Mp3ParsedData? ParsedData { get; set; } = new();
}

/// <summary>
/// Information from filename
/// </summary>
public class Mp3ParsedData
{
    /// <summary>
    /// Paragraph Id
    /// </summary>
    public ParaId ParaId { get; set; }

    /// <summary>
    /// File number (first 4 digits)
    /// </summary>
    public string FileNumber { get; set; } = "";

    /// <summary>
    /// Voice
    /// </summary>
    public VoiceTypeEnum Voice { get; set; }

    /// <summary>
    /// Path to file (without base bucket) 
    /// </summary>
    public string Path { get; set; } = "";
}