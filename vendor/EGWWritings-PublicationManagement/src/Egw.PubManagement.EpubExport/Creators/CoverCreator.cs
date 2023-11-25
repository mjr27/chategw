using Egw.PubManagement.EpubExport.Extensions;
using Egw.PubManagement.EpubExport.Services;
using Egw.PubManagement.Persistence.Entities;


namespace Egw.PubManagement.EpubExport.Creators;

/// <summary>
/// Cover creator
/// </summary>
public class CoverCreator
{
    private readonly ICoverFetcher _coverFetcher;
    private readonly TemplateService _template;

    /// <summary>
    /// Cover creator
    /// </summary>
    /// <param name="coverFetcher"></param>
    /// <param name="template"></param>
    public CoverCreator(ICoverFetcher coverFetcher, TemplateService template)
    {
        _coverFetcher = coverFetcher;
        _template = template;
    }

    /// <summary>
    /// Creates the cover page
    /// </summary>
    /// <param name="publication"></param>
    /// <param name="basePath"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<bool> Create(Publication publication, string basePath, CancellationToken ct)
    {
        string? pathToCover = await TryDownloadCover(publication, basePath, ct);
        if (pathToCover is null)
        {
            return false;
        }

        var model = new { publicationTitle = publication.Title };

        await _template.RenderToFile(
            Path.Combine(basePath, "EPUB/xhtml/cover.xhtml"),
            "cover",
            model);
        string css = await typeof(EpubGenerator).Assembly.ReadTextResource("cover.css", ct) ?? "";
        await File.WriteAllTextAsync(Path.Combine(basePath, "EPUB", "css", "cover.css"), css, ct);
        return true;
    }

    private async Task<string?> TryDownloadCover(Publication publication, string baseFolder, CancellationToken ct)
    {
        byte[]? cover = await _coverFetcher.FetchCover(publication.PublicationId, ct);
        if (cover is null)
        {
            return null;
        }

        string path = Path.Combine(baseFolder, "EPUB", "img", "cover.jpg");
        await File.WriteAllBytesAsync(path, cover, ct);
        return path;
    }
}