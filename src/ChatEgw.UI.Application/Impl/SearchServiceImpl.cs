using ChatEgw.UI.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pgvector;

namespace ChatEgw.UI.Application.Impl;

internal class SearchServiceImpl : ISearchService
{
    private readonly IRawSearchEngine _rawSearchEngine;
    private readonly IQueryPreprocessService _queryPreprocessService;
    private readonly IQueryEmbeddingService _queryEmbeddingService;
    private readonly IQuestionAnsweringService _questionAnsweringService;
    private readonly ILogger<SearchServiceImpl> _logger;
    private readonly int _limit;

    public SearchServiceImpl(
        IConfiguration configuration,
        IRawSearchEngine rawSearchEngine,
        IQueryPreprocessService queryPreprocessService,
        IQueryEmbeddingService queryEmbeddingService,
        IQuestionAnsweringService questionAnsweringService,
        ILogger<SearchServiceImpl> logger)
    {
        _rawSearchEngine = rawSearchEngine;
        _queryPreprocessService = queryPreprocessService;
        _queryEmbeddingService = queryEmbeddingService;
        _questionAnsweringService = questionAnsweringService;
        _logger = logger;
        _limit = configuration.GetValue<int>("Search:TopN");
    }

    public async Task<AnsweringResponse> Search(
        SearchTypeEnum searchType,
        string query,
        SearchFilterRequest filter,
        CancellationToken cancellationToken)
    {
        PreprocessedQueryResponse preprocessedInfo =
            await _queryPreprocessService.PreprocessQuery(query, cancellationToken);
        bool isQuestion = searchType switch
        {
            SearchTypeEnum.Auto => preprocessedInfo.IsQuestion,
            SearchTypeEnum.AiSearch => true,
            _ => false
        };
        AnsweringResponse response = isQuestion
            ? await AiSearch(preprocessedInfo.NormalizedQuery, preprocessedInfo.References, preprocessedInfo.Entities,
                filter, cancellationToken)
            : await KeywordSearch(preprocessedInfo.NormalizedQuery, filter, cancellationToken);
        response.UpdatedQuery = preprocessedInfo.NormalizedQuery;
        return response;
    }

    private async Task<AnsweringResponse> AiSearch(string query,
        IReadOnlyCollection<PreprocessedPublicationReference> references,
        IReadOnlyCollection<PreprocessedEntity> entities,
        SearchFilterRequest filter,
        CancellationToken cancellationToken)
    {
        Vector embedding = await _queryEmbeddingService.Embed(query, cancellationToken);
        List<SearchResultDto> result = await _rawSearchEngine.SearchEmbeddings(
            embedding,
            _limit,
            references,
            entities,
            filter,
            cancellationToken);
        _logger.LogInformation("Found {Count} results", result.Count);
        return new AnsweringResponse(
            true,
            await _questionAnsweringService.AnswerQuestions(query, result, cancellationToken));
    }

    private async Task<AnsweringResponse> KeywordSearch(
        string query,
        SearchFilterRequest filter,
        CancellationToken cancellationToken)
    {
        List<SearchResultDto> data = await _rawSearchEngine.SearchFts(query, _limit, filter, cancellationToken);
        return new AnsweringResponse(false,
            data.Select((r, i) => new AnswerResponse
            {
                Id = i + 1,
                Uri = r.Uri,
                Score = (float)r.Distance,
                Snippet = r.Snippet,
                Content = r.Content,
                ReferenceCode = r.ReferenceCode,
                Answer = null
            }).ToList());
    }
}