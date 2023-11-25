using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Services.Metadata.Enrichers;

internal class BibleEnricher : IMetadataEnricher
{
    private string _currentBook = "";
    private int _currentChapter = 1;
    private int _currentVerse = 1;

    public void EnrichMetadata(Publication publication, Paragraph paragraph, ParagraphMetadata metadata)
    {
        try
        {
            switch (paragraph.HeadingLevel)
            {
                case 1:
                    break;
                case 2:
                    {
                        IWemlNode node = paragraph.DeserializedContent();
                        _currentBook = node.TextContent().Trim().ToUpperInvariant();
                        _currentChapter = 1;
                        _currentVerse = 1;
                        metadata.WithBible(_currentBook, 0, Array.Empty<int>());
                        break;
                    }
                case 3:
                    {
                        IWemlNode node = paragraph.DeserializedContent();
                        _currentChapter = int.Parse(node.TextContent());
                        _currentVerse = 1;
                        metadata.WithBible(_currentBook, _currentChapter, Array.Empty<int>());
                        break;
                    }
                case 4:
                    {
                        IWemlNode node = paragraph.DeserializedContent();
                        _currentVerse = int.Parse(node.TextContent());
                        break;
                    }
                default:
                    metadata.WithBible(_currentBook, _currentChapter, new[] { _currentVerse });
                    break;
            }
        }
        catch (FormatException e)
        {
            throw new FormatException($"{e.Message} in {paragraph.ParaId}");
        }
    }
}