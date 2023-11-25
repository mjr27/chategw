using Egw.PubManagement.EpubExport.Extensions;
using Egw.PubManagement.EpubExport.Services;
using Egw.PubManagement.Persistence.Entities;


namespace Egw.PubManagement.EpubExport.Creators;

/// <summary>
/// About page creator
/// </summary>
public class AboutCreator
{
    private readonly TemplateService _template;

    /// <summary>
    /// Default constructor
    /// </summary>
    public AboutCreator(TemplateService template)
    {
        _template = template;
    }

    /// <summary>
    /// Creates the about page
    /// </summary>
    /// <param name="publication"></param>
    /// <returns></returns>
    public async Task<bool> Create(Publication publication)
    {
        string resourceName = $"about.{publication.LanguageCode}";
        if (!typeof(EpubGenerator).Assembly.ResourceExists($"{resourceName}.liquid"))
        {
            return false;
        }

        var model = new { publicationTitle = publication.Title };
        await _template.RenderToFile("EPUB/xhtml/aboutbook.xhtml", resourceName, model);
        return true;
    }
}