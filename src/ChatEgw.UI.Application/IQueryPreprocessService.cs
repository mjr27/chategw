using ChatEgw.UI.Application.Models;

namespace ChatEgw.UI.Application;

internal interface IQueryPreprocessService
{
    Task<PreprocessedQueryResponse> IsQuestion(string query, CancellationToken cancellationToken);
}