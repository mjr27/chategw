namespace ChatEgw.UI.Persistence;

public class SearchNode
{
    public required string Id { get; set; }
    public required bool IsFolder { get; set; }
    public required string Title { get; set; }
    public required bool IsEgw { get; set; }
    public required string? ParentId { get; set; }
    public required int GlobalOrder { get; set; }
    public required DateOnly? Date { get; set; }
    public List<string> Children { get; set; } = new();
    public List<SearchParagraph> Paragraphs { get; set; } = new();
}