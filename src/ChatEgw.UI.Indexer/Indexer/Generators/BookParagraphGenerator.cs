using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Entities.Metadata;

namespace ChatEgw.UI.Indexer.Indexer.Generators;

internal sealed class BookParagraphGenerator : BaseParagraphGenerator
{
    public BookParagraphGenerator(PublicationDbContext db, IndexPublicationDto publication) : base(db, publication)
    {
    }

    protected override IEnumerable<string> GetReferenceCodes(ParagraphMetadata? paragraphMetadata)
    {
        if (paragraphMetadata?.Pagination is null)
        {
            yield break;
        }

        PaginationMetaData? meta = paragraphMetadata.Pagination;
        yield return meta.Section;
        if (meta.Paragraph == 0) yield break;
        yield return $"{meta.Section} {meta.Paragraph}";
    }

    protected override IEnumerable<ParagraphModel> FetchParagraphs(IQueryable<Paragraph> paragraphs)
    {
        return paragraphs
            .Where(r => r.HeadingLevel == 0)
            .Select(r=> new
            {
                r.ParaId,
                r.Content,
                r.Metadata
            })
            .AsEnumerable()
            .Select(r => new ParagraphModel(
                r.ParaId,
                r.Content,
                r.Metadata,
                GetReferences(r.Metadata).ToList()
            ));
    }
}