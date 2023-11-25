using Egw.PubManagement.EpubExport.Extensions;
using Egw.PubManagement.EpubExport.Services;
using Egw.PubManagement.EpubExport.Types;
using Egw.PubManagement.Persistence.Entities;

namespace Egw.PubManagement.EpubExport.Creators;

/// <summary> Toc.ncx creator </summary>
public class TocNcxCreator
{
    private readonly TemplateService _template;

    /// <summary>
    /// Default constructor
    /// </summary>
    public TocNcxCreator(TemplateService template)
    {
        _template = template;
    }

    /// <summary> Creates the toc.ncx file </summary>
    public async Task<bool> Create(Guid epubGuid, Publication publication, List<EpubChapter> chapters)
    {
        int number = 1;
        foreach (EpubChapter chapter in chapters)
        {
            chapter.PlayOrder = number++;
            foreach (EpubPageInfo page in chapter.Pages)
            {
                page.PlayOrder = number++;
            }
        }

        bool isPagesFound = chapters.Any(chapter => chapter.Pages.Any());
        var model = new
        {
            publication,
            epubGuid,
            chapters,
            isPagesFound,
            tree = chapters.ToTree()
        };

        await _template.RenderToFile("EPUB/toc.ncx", "toc-ncx", model);
        return true;
    }
}