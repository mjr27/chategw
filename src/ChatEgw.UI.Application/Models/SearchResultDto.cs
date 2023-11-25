namespace ChatEgw.UI.Application.Models;

public class SearchResultDto
{
    public required long Id { get; set; }
    public required string ReferenceCode { get; set; }
    public required string Snippet { get; set; }
    public required string Content { get; set; }
    public required Uri? Uri { get; set; }
    public required double Distance { get; set; }
}