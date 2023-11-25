using ChatEgw.UI.Application.Models;

namespace ChatEgw.UI.Application;

internal interface IQuestionAnsweringService
{
    Task<List<AnswerResponse>> AnswerQuestions(string query,
        IReadOnlyCollection<SearchResultDto> searchResults,
        CancellationToken cancellationToken);
}