namespace ChatEgw.UI.Application.Models;

public class AnswerResponse
{
    public required int Id { get; init; }
    public required string ReferenceCode { get; set; }
    public required Uri? Uri { get; set; }
    public required string? Answer { get; set; }
    
    public required string Snippet { get; set; }
    public required string Content { get; set; }
    public required float Score { get; set; }
}