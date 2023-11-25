using Egw.PubManagement.EpubExport.Services;
using Egw.PubManagement.Persistence.Entities;

namespace Egw.PubManagement.EpubExport.Creators;

/// <summary> Title page creator </summary>
public class TitlePageCreator
{
    private readonly TemplateService _template;

    /// <summary> Default constructor </summary>
    public TitlePageCreator(TemplateService template)
    {
        _template = template;
    }

    /// <summary> Creates the title page </summary>
    public async Task<bool> Create(Publication publication)
    {
        var model = new
        {
            publicationTitle = publication.Title,
            author = publication.Author,
            isbn = publication.Isbn ?? "",
            year = publication.PublicationYear ?? 0
        };
        await _template.RenderToFile("EPUB/xhtml/titlepage.xhtml", "title-page", model);
        return true;
    }
}