using ChatEgw.UI.Application.Models;

namespace ChatEgw.UI.Application;

public interface ISearchService
{
    Task<AnsweringResponse> Search(
        SearchTypeEnum searchType,
        string query,
        SearchFilterRequest filter,
        CancellationToken cancellationToken);
}