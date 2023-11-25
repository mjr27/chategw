using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Services.Metadata.Enrichers;

internal class BookPaginationEnricher : IMetadataEnricher
{
    private int _paragraph;
    private string _pageNumber;

    public BookPaginationEnricher()
    {
        _pageNumber = "";
        _paragraph = 1;
    }

    public void EnrichMetadata(Publication publication, Paragraph paragraph, ParagraphMetadata metadata)
    {
        IWemlNode node = paragraph.DeserializedContent();
        switch (paragraph.HeadingLevel)
        {
            case null:
                WemlPageBreakElement pageElement = node
                    .DescendantsAndSelf()
                    .OfType<WemlPageBreakElement>()
                    .Last();
                _paragraph = 1;
                _pageNumber = pageElement.Page;
                break;
            case 0:
                {
                    if (paragraph.IsReferenced)
                    {
                        metadata.WithPagination(_pageNumber, _paragraph++);
                    }

                    WemlPageBreakElement? lastPageElement = node
                        .DescendantsAndSelf()
                        .OfType<WemlPageBreakElement>()
                        .LastOrDefault();

                    if (lastPageElement is not null)
                    {
                        _pageNumber = lastPageElement.Page;
                        _paragraph = 1;
                    }

                    break;
                }
            case >= 1 and <= 6:
                {
                    WemlPageBreakElement? lastPageElement = node
                        .DescendantsAndSelf()
                        .OfType<WemlPageBreakElement>()
                        .LastOrDefault();
                    if (lastPageElement is not null)
                    {
                        _pageNumber = lastPageElement.Page;
                        _paragraph = 1;
                    }
                    metadata.WithPagination(_pageNumber, 0);
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException($"invalid heading level: {paragraph.HeadingLevel.Value}");
        }
    }
}