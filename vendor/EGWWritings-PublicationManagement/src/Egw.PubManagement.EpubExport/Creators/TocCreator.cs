using Egw.PubManagement.EpubExport.Extensions;
using Egw.PubManagement.EpubExport.Services;
using Egw.PubManagement.EpubExport.Types;
using Egw.PubManagement.Persistence.Entities;

namespace Egw.PubManagement.EpubExport.Creators;

/// <summary>
/// TOC Creator
/// </summary>
public class TocCreator
{
    private readonly TemplateService _template;

    /// <summary> Default constructor </summary>
    public TocCreator(TemplateService template)
    {
        _template = template;
    }

    /// <summary> Creates the toc.xhtml file </summary>
    public async Task<bool> Create(Publication publication, List<EpubChapter> chapters)
    {
        bool isPagesFound = chapters.Any(chapter => chapter.Pages.Any());

        await _template.RenderToFile("EPUB/toc.xhtml", "toc",
            new { chapters, publicationTitle = publication.Title, isPagesFound, tree = chapters.ToTree() });

        if (!isPagesFound)
        {
            return true;
        }

        await _template.RenderToFile("EPUB/xhtml/page-map.xml", "page-map",
            new { pages = chapters.SelectMany(r => r.Pages).ToList() });
        return true;
    }
}