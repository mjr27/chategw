using Egw.PubManagement.EpubExport.Services;
using Egw.PubManagement.EpubExport.Types;
using Egw.PubManagement.Persistence.Entities;


namespace Egw.PubManagement.EpubExport.Creators;

/// <summary>
/// package.opf creator
/// </summary>
public class PackageOpfCreator
{
    private readonly TemplateService _template;

    /// <summary> Default constructor </summary>
    public PackageOpfCreator(TemplateService template)
    {
        _template = template;
    }

    /// <summary> Creates the package.opf file </summary>
    public async Task<bool> Create(
        Guid epubGuid,
        DateTime createdAtUtc,
        Publication publication,
        List<EpubInnerFileInfo> files)
    {
        var model = new
        {
            publication, epubGuid, generationTime = createdAtUtc.ToString(@"yyyy-MM-dd\Thh:mm:ss\Z"), files
        };
        await _template.RenderToFile(
            "EPUB/package.opf",
            "package",
            model);
        return true;
    }
}