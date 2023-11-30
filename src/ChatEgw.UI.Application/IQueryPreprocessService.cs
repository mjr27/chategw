using ChatEgw.UI.Application.Models;

namespace ChatEgw.UI.Application;

internal interface IQueryPreprocessService
{
    Task<PreprocessedQueryResponse> PreprocessQuery(string query, CancellationToken cancellationToken);
}