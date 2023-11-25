using Egw.PubManagement.EpubExport.Types;
using Egw.PubManagement.Persistence.Entities;

using Fluid;

using Microsoft.Extensions.FileProviders;

namespace Egw.PubManagement.EpubExport.Services;

internal class EpubTemplateOptions : TemplateOptions
{
    public EpubTemplateOptions()
    {
        Filters.AddFilter("decode", CustomTemplateFilters.Decode);
        Filters.AddFilter("html_to_utf", CustomTemplateFilters.HtmlToUtf);
        FileProvider = new EmbeddedFileProvider(
            typeof(EpubGenerator).Assembly,
            "Egw.PubManagement.EpubExport.Templates"
        );

        MemberAccessStrategy.Register<Paragraph>();
        MemberAccessStrategy.Register<Publication>();
        MemberAccessStrategy.Register<PublicationAuthor>();
        MemberAccessStrategy.Register<EpubChapter>();
        MemberAccessStrategy.Register<EpubInnerFileInfo>();
        MemberAccessStrategy.Register<EpubFootNoteInfo>();
        MemberAccessStrategy.Register<EpubPageInfo>();
        MemberAccessStrategy.Register<EpubTreeItem>();

        MemberAccessStrategy.MemberNameStrategy = MemberNameStrategies.CamelCase;
    }
}