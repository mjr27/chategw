using System.IO.Compression;

using Egw.PubManagement.EpubExport.Creators;
using Egw.PubManagement.EpubExport.Services;
using Egw.PubManagement.EpubExport.Types;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Fizzler.Systems.HtmlAgilityPack;

using HtmlAgilityPack;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Egw.PubManagement.EpubExport;

/// <summary>
/// Weml to Epub converter.
/// </summary>
public class EpubGenerator
{
    private readonly PublicationDbContext _db;
    private readonly ICoverFetcher _coverFetcher;
    private readonly ILoggerFactory _lf;
    private readonly ILogger<EpubGenerator> _logger;

    private readonly List<EpubMetaFile> _metaFiles = new()
    {
        new EpubMetaFile { File = "cover.xhtml", Name = "cover", Title = "Cover", Order = 1 },
        new EpubMetaFile { File = "titlepage.xhtml", Name = "titlepage", Title = "Title Page", Order = 2 },
        new EpubMetaFile { File = "toc.xhtml", Name = "toc", Title = "Table of Content", Order = 3 },
        new EpubMetaFile { File = "aboutbook.xhtml", Name = "aboutbook", Title = "About book", Order = 4 }
    };


    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="coverFetcher"></param>
    /// <param name="lf"></param>
    public EpubGenerator(PublicationDbContext db,
        ICoverFetcher coverFetcher,
        ILoggerFactory lf)
    {
        _db = db;
        _coverFetcher = coverFetcher;
        _lf = lf;
        _logger = _lf.CreateLogger<EpubGenerator>();
    }

    /// <summary>
    /// Creates a epub file
    /// </summary>
    /// <param name="ct"></param>
    /// <param name="id">Unique ID of the epub file</param>
    /// <param name="createdAt">Date of generation</param>
    /// <param name="publicationId">Publication ID</param>
    /// <param name="temporaryFolder">Folder to place temporary files</param>
    /// <param name="output">Output filename</param>
    /// <returns>file path</returns>
    public async Task Create(
        int publicationId,
        Guid id,
        DateTime createdAt,
        DirectoryInfo temporaryFolder,
        FileInfo output,
        CancellationToken ct)
    {
        Publication? publication = await _db.Publications
            .Include(model => model.Author)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.PublicationId == publicationId, ct);
        if (publication is null)
        {
            throw new ArgumentException($"Publication #{publicationId} not found");
        }

        _logger.LogInformation("Create EPUB for: {PublicationId} - {Title}", publication.PublicationId,
            publication.Title);

        Language? language =
            await _db.Languages.FirstOrDefaultAsync(r => r.Code.ToLower() == publication.LanguageCode.ToLower(), ct);
        var template = new TemplateService(temporaryFolder.FullName, _lf.CreateLogger<TemplateService>());
        if (language is not null)
        {
            template.Language = language.Bcp47Code;
            template.LangDir = language.IsRightToLeft ? "rtl" : "ltr";
        }

        var structureCreator = new StructureCreator(temporaryFolder.FullName);
        await structureCreator.Create(publication, ct);

        List<PublicationChapter> chapters = await GetPublicationChapters(publication, ct);

        var coverCreator = new CoverCreator(_coverFetcher, template);
        if (!await coverCreator.Create(publication, temporaryFolder.FullName, ct))
        {
            _metaFiles.Remove(_metaFiles.First(r => r.Name == "cover"));
        }

        var aboutCreator = new AboutCreator(template);
        if (!await aboutCreator.Create(publication))
        {
            _metaFiles.Remove(_metaFiles.First(r => r.Name == "aboutbook"));
        }

        var titlePageCreator = new TitlePageCreator(template);
        await titlePageCreator.Create(publication);

        List<EpubChapter> preparedChapters = PrepareChapters(chapters, _metaFiles);

        var tocCreator = new TocCreator(template);
        var contentCreator = new ContentCreator(_db, template);
        await contentCreator.Create(publication, preparedChapters,  ct);
        await tocCreator.Create(publication, preparedChapters);

        List<EpubInnerFileInfo> innerFiles = GetInnerFiles(temporaryFolder.FullName);

        var tocNcxCreator = new TocNcxCreator(template);
        await tocNcxCreator.Create(id, publication, preparedChapters);

        var packageOpfCreator = new PackageOpfCreator(template);
        await packageOpfCreator.Create(id, createdAt, publication, innerFiles);
        ToZip(temporaryFolder, output);
    }

    private List<EpubInnerFileInfo> GetInnerFiles(string basePath)
    {
        var files = Directory
            .GetFileSystemEntries(Path.Combine(basePath, "EPUB"), "*", SearchOption.AllDirectories)
            .Where(r => !File.GetAttributes(r).HasFlag(FileAttributes.Directory))
            .OrderBy(s =>
            {
                var f = new FileInfo(s);
                return Orders.Contains(f.Name) ? Array.IndexOf(Orders, f.Name) : int.MaxValue;
            })
            .ThenBy(s => s)
            .Select(r => FillFileInfo(basePath, r))
            .ToList();
        return files;
    }

    private EpubInnerFileInfo FillFileInfo(string basePath, string file)
    {
        var item = new EpubInnerFileInfo
        {
            File = file
                .Replace(Path.Combine(basePath, "EPUB"), "")
                .Replace('\\', '/')
                .TrimStart('/'),
            Extension = Path.GetExtension(file).Trim('.').ToLower(),
            Name = Path.GetFileNameWithoutExtension(file).ToLower()
        };

        switch (item.Extension)
        {
            case "xml":
                item.MediaType = "application/oebps-page-map+xml";
                if (item.Name == "page-map")
                {
                    item.Id = "map";
                }

                item.Id = $"{item.Name.ToLower()}";
                break;
            case "jpg":
                item.MediaType = "image/jpeg";
                item.Id = $"{item.Name.ToLower()}-image";
                break;
            case "css":
                item.MediaType = "text/css";
                item.Id = $"{item.Name.ToLower()}-css";
                break;
            case "xhtml":
                item.MediaType = "application/xhtml+xml";
                item.Id = $"{item.Name.ToLower()}";
                break;
            case "ncx":
                item.MediaType = "application/x-dtbncx+xml";
                item.Id = $"{item.Name.ToLower()}-ncx";
                break;
            default:
                _logger.LogError("Unknown file extension: {File}", file);
                break;
        }

        return item;
    }

    private async Task<List<PublicationChapter>> GetPublicationChapters(Publication publication, CancellationToken ct)
    {
        List<PublicationChapter> chapters = await _db.PublicationChapters
            .AsNoTracking()
            .Where(r => r.PublicationId == publication.PublicationId)
            .Where(r => r.Level > 1)
            .OrderBy(r => r.Order)
            .ToListAsync(ct);
        return chapters;
    }

    private List<EpubChapter> PrepareChapters(List<PublicationChapter> chapters, List<EpubMetaFile> metaFiles)
    {
        var preparedChapters = new List<EpubChapter>();
        int i = 1;

        foreach (EpubMetaFile file in metaFiles)
        {
            preparedChapters.Add(new EpubChapter
            {
                FileNumber = i,
                Id = file.Name,
                FileNumberFormatted = i.ToString("D4"),
                Title = metaFiles.FirstOrDefault(m => m.Name == file.Name)?.Title ?? file.Name,
                Level = 2,
                Order = 0,
                EndOrder = 0,
                File = Path.GetFileName(file.File)
            });
            i++;
        }

        foreach (PublicationChapter chapter in chapters)
        {
            preparedChapters.Add(new EpubChapter
            {
                FileNumber = i,
                File = $"content{i:D4}.xhtml",
                Id = chapter.ChapterId.ToString(),
                ParaId = chapter.ParaId,
                FileNumberFormatted = i.ToString("D4"),
                Title = RemoveSupFromChapterTitle(chapter.Title),
                Level = chapter.Level,
                PublicationId = chapter.PublicationId,
                Order = chapter.Order,
                EndOrder = chapter.ContentEndOrder
            });
            i++;
        }

        return preparedChapters;
    }

    private string RemoveSupFromChapterTitle(string title)
    {
        var html = new HtmlDocument();
        html.LoadHtml(title);
        List<HtmlNode> nodes = html.DocumentNode.QuerySelectorAll("sup").ToList();
        if (nodes.Any())
        {
            foreach (HtmlNode nodeToRemove in nodes)
            {
                HtmlNode? parent = nodeToRemove.ParentNode;
                parent.RemoveChild(nodeToRemove);
            }
        }

        html.OptionWriteEmptyNodes = true;
        return html.DocumentNode.WriteContentTo();
    }

    private static readonly string[] Orders =
    {
        "mimetype", "container.xml", "package.opf", "cover.css", "cover.jpg", "cover.xhtml", "titlepage.xhtml",
        "toc.ncx", "toc.xhtml", "aboutbook.xhtml", "bookstyles.css"
    };

    private void ToZip(
        DirectoryInfo folder,
        FileSystemInfo output)
    {
        IOrderedEnumerable<FileInfo> allFiles = folder
            .GetFiles("*.*", SearchOption.AllDirectories)
            .OrderBy(f => Orders.Contains(f.Name) ? Array.IndexOf(Orders, f.Name) : int.MaxValue)
            .ThenBy(r => r.FullName);
        using FileStream archiveStream = File.Open(output.FullName, FileMode.Create);
        using var epub = new ZipArchive(archiveStream, ZipArchiveMode.Create, true);
        var lastWriteTime = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
        foreach (FileInfo fileInfo in allFiles)
        {
            CompressionLevel level = fileInfo.FullName.EndsWith("/mimetype")
                ? CompressionLevel.NoCompression
                : CompressionLevel.SmallestSize;
            byte[] fileData = File.ReadAllBytes(fileInfo.FullName);
            string path = fileInfo.FullName[(folder.FullName.Length + 1)..];
            ZipArchiveEntry entry = epub.CreateEntry(path, level);
            entry.LastWriteTime = lastWriteTime;
            using Stream zipStream = entry.Open();
            zipStream.Write(fileData);
        }

        archiveStream.Flush();
    }
}