namespace ChatEgw.UI.Application.Models;

public class PreprocessedPublicationReference
{
    public required string Publication { get; set; }
    public int? Page { get; set; }
    public int? EndPage { get; set; }
    public int? Chapter { get; set; }
    public int? EndChapter { get; set; }
}