using ChatEgw.UI.Application.Models;
using Pgvector;

namespace ChatEgw.UI.Application;

internal interface IRawSearchEngine
{
    Task<List<SearchResultDto>> SearchEmbeddings(
        Vector query,
        int limit,
        SearchFilterRequest filter,
        CancellationToken cancellationToken);

    Task<List<SearchResultDto>> SearchFts(
        string query,
        int limit,
        SearchFilterRequest filter,
        CancellationToken cancellationToken);
}