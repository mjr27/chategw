namespace ChatEgw.UI.Application.Models;

public class AnsweringResponse(bool isAiResponse, IReadOnlyCollection<AnswerResponse> answers)
{
    public bool IsAiResponse { get; } = isAiResponse;
    public string UpdatedQuery { get; set; } = "";
    public IReadOnlyCollection<AnswerResponse> Answers { get; } = answers;
}