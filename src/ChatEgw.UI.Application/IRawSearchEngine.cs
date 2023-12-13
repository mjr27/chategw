using ChatEgw.UI.Application.Models;
using Pgvector;

namespace ChatEgw.UI.Application;

internal interface IRawSearchEngine
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="limit">Limit</param>
    /// <param name="references">List of references to include</param>
    /// <param name="entities">List of entities to include</param>
    /// <param name="filter">Filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<List<SearchResultDto>> SearchEmbeddings(Vector query,
        int limit,
        IReadOnlyCollection<PreprocessedPublicationReference> references,
        IReadOnlyCollection<PreprocessedEntity> entities,
        SearchFilterRequest filter,
        CancellationToken cancellationToken);

    Task<List<SearchResultDto>> SearchFts(
        string query,
        int limit,
        SearchFilterRequest filter,
        CancellationToken cancellationToken);
}