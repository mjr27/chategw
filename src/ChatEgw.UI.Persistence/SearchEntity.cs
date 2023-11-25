using System.ComponentModel.DataAnnotations;

namespace ChatEgw.UI.Persistence;

public class SearchEntity
{
    [Key] public long Id { get; set; }
    public long SearchChunkId { get; set; }

    /// <summary> Search chunk navigation property </summary>
    public SearchChunk SearchChunk { get; set; } = null!;

    public required SearchEntityTypeEnum Type { get; set; }
    public required string Content { get; set; }
}