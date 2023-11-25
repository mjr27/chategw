using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Application.Services.Metadata.Enrichers;

internal class TopicalEnricher : IMetadataEnricher
{
    private string _word;
    private int _paragraphIndex;

    public TopicalEnricher()
    {
        _word = "";
        _paragraphIndex = 1;
    }

    public void EnrichMetadata(Publication publication, Paragraph paragraph, ParagraphMetadata metadata)
    {
        switch (paragraph.HeadingLevel)
        {
            case 4:
                IWemlNode node = paragraph.DeserializedContent();
                _word = node.DescendantsAndSelf().OfType<WemlEntityElement>()
                            .Where(r => r.EntityType == WemlEntityType.TopicWord)
                            .Select(r => r.TextContent().Trim().ToUpperInvariant())
                            .Select(s => string.IsNullOrWhiteSpace(s) ? null : s)
                            .FirstOrDefault()
                        ?? node.TextContent().Trim().ToUpperInvariant();
                if (_word.Length > 400)
                {
                    throw new ConflictProblemDetailsException("Too long topical index word:" + _word);
                }

                _paragraphIndex = 1;
                metadata.WithPagination(_word, _paragraphIndex++);
                break;
            case 0 or > 4:
                metadata.WithPagination(_word, _paragraphIndex++);
                break;
        }
    }
}