using System.Text.RegularExpressions;

using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Services.Metadata.Enrichers;

internal class ScriptureIndexPaginationEnricher : IMetadataEnricher
{
    private string _currentBook = "";
    private int _currentChapter = 1;
    private int[] _verses = Array.Empty<int>();


    public void EnrichMetadata(Publication publication, Paragraph paragraph, ParagraphMetadata metadata)
    {
        switch (paragraph.HeadingLevel)
        {
            case 1:
                break;
            case 2:
                {
                    IWemlNode node = paragraph.DeserializedContent();
                    _currentBook = node.TextContent().Trim().ToUpperInvariant();
                    _currentChapter = 0;
                    _verses = Array.Empty<int>();
                    metadata.WithBible(_currentBook, 0, Array.Empty<int>());
                    break;
                }
            case 3:
                {
                    IWemlNode node = paragraph.DeserializedContent();
                    _currentChapter = int.Parse(node.TextContent());
                    _verses = Array.Empty<int>();
                    metadata.WithBible(_currentBook, _currentChapter, Array.Empty<int>());
                    break;
                }
            case 4:
                {
                    IWemlNode node = paragraph.DeserializedContent();
                    _verses = ExtractRange(node.TextContent());
                    metadata.WithBible(_currentBook, _currentChapter, _verses);
                    break;
                }
            default:
                metadata.WithBible(_currentBook, _currentChapter, _verses);
                break;
        }
    }

    private static readonly Regex ReRange = new(@"(\d+)\s*-\s*(\d+)");


    private static int[] ExtractRange(string range)
    {
        range = range.Trim();
        if (int.TryParse(range, out int singleItem))
        {
            return new[] { singleItem };
        }

        Match m1 = ReRange.Match(range);
        if (m1.Success)
        {
            int startIdx = int.Parse(m1.Groups[1].Value);
            int endIdx = int.Parse(m1.Groups[2].Value);
            return Enumerable.Range(startIdx, endIdx - startIdx + 1).ToArray();
        }

        // commas
        if (range.All(c => char.IsDigit(c) || char.IsWhiteSpace(c) || c == ','))
        {
            return range
                .Split()
                .Select(s => s.Trim(','))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s =>
                    int.TryParse(s, out int i)
                        ? i
                        : throw new InvalidDataException($"Cannot parse {range}"))
                .ToArray();
        }
        return Array.Empty<int>();
    }
}