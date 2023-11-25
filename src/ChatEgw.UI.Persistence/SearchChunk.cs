using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace ChatEgw.UI.Persistence;

public class SearchChunk
{
    [Key] public long Id { get; set; }
    public required long ParagraphId { get; set; }
    public SearchParagraph Paragraph { get; set; } = null!;
    public required string Content { get; set; }
    [Column(TypeName = "vector(768)")] public required Vector Embedding { get; set; }

    public List<SearchEntity> Entities { get; set; } = new();
}