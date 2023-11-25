namespace ChatEgw.UI.Application.Models;

public class AnsweringResponse
{
    public bool IsAiResponse { get; }
    public string UpdatedQuery { get; set; } = "";
    public IReadOnlyCollection<AnswerResponse> Answers { get; }

    public AnsweringResponse(bool isAiResponse, IReadOnlyCollection<AnswerResponse> answers)
    {
        IsAiResponse = isAiResponse;
        Answers = answers;
    }
}