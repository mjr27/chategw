using ChatEgw.UI.Application.Models;

namespace ChatEgw.UI.Application;

public interface IInstructGenerationService
{
    public IAsyncEnumerable<string> AutoComplete(string query, List<AnswerResponse> answers,
        CancellationToken cancellationToken);
}