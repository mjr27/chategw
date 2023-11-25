using System.Text.Json;
using ChatEgw.UI.Application.Models;
using Microsoft.Extensions.Configuration;
using Pgvector;

namespace ChatEgw.UI.Application.Impl;

internal class SearchServiceImpl : ISearchService
{
    private readonly IRawSearchEngine _rawSearchEngine;
    private readonly IQueryPreprocessService _queryPreprocessService;
    private readonly IQueryEmbeddingService _queryEmbeddingService;
    private readonly IQuestionAnsweringService _questionAnsweringService;
    private readonly int _limit;

    public SearchServiceImpl(
        IConfiguration configuration,
        IRawSearchEngine rawSearchEngine,
        IQueryPreprocessService queryPreprocessService,
        IQueryEmbeddingService queryEmbeddingService,
        IQuestionAnsweringService questionAnsweringService
    )
    {
        _rawSearchEngine = rawSearchEngine;
        _queryPreprocessService = queryPreprocessService;
        _queryEmbeddingService = queryEmbeddingService;
        _questionAnsweringService = questionAnsweringService;
        _limit = configuration.GetValue<int>("Search:TopN");
    }

    public async Task<AnsweringResponse> Search(SearchTypeEnum searchType,
        string query,
        SearchFilterRequest filter,
        CancellationToken cancellationToken)
    {
        PreprocessedQueryResponse preprocessedInfo =
            await _queryPreprocessService.IsQuestion(query, cancellationToken);
        bool isQuestion = searchType switch
        {
            SearchTypeEnum.Auto => preprocessedInfo.IsQuestion,
            SearchTypeEnum.AiSearch => true,
            _ => false
        };
        AnsweringResponse response = isQuestion
            ? await AiSearch(preprocessedInfo.NormalizedQuery, filter, cancellationToken)
            : await KeywordSearch(preprocessedInfo.NormalizedQuery, filter, cancellationToken);
        response.UpdatedQuery = preprocessedInfo.NormalizedQuery;
        return response;
    }

    private async Task<AnsweringResponse> AiSearch(
        string query,
        SearchFilterRequest filter,
        CancellationToken cancellationToken)
    {
        Vector embedding = await _queryEmbeddingService.Embed(query, cancellationToken);
        List<SearchResultDto> result = await _rawSearchEngine.SearchEmbeddings(
            embedding,
            _limit,
            filter,
            cancellationToken);
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
        Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
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