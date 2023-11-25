using System.ComponentModel.DataAnnotations;

namespace ChatEgw.UI.Persistence;

public class SearchParagraph
{
    [Key] public required long Id { get; set; }
    public required string NodeId { get; set; }
    public SearchNode Node { get; set; } = null!;
    public required string RefCode { get; set; }
    public required bool IsEgw { get; set; }
    public required Uri? Uri { get; set; }
    public required string Content { get; set; }
    public List<SearchChunk> Entities { get; set; } = new();
    public List<SearchParagraphReference> References { get; set; } = new();
}