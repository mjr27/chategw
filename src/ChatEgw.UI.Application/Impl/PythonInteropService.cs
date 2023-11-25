using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatEgw.UI.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pgvector;

namespace ChatEgw.UI.Application.Impl;

internal class PythonInteropServiceImpl : IQuestionAnsweringService, IQueryEmbeddingService, IQueryPreprocessService
{
    private readonly HttpClient _httpClient;

    private readonly ILogger<PythonInteropServiceImpl> _logger;

    // private readonly Uri _baseUri;
    private readonly Uri _embedUri;
    private readonly Uri _answerQuestionsUri;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Uri _preprocessQuestionsUri;

    public PythonInteropServiceImpl(
        HttpClient httpClient,
        IOptions<JsonOptions> options,
        IConfiguration configuration,
        ILogger<PythonInteropServiceImpl> logger
    )
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = options.Value.JsonSerializerOptions;
        var baseUri = new Uri(configuration.GetConnectionString("PythonApi")!);
        _embedUri = new Uri(baseUri, "/api/embed");
        _answerQuestionsUri = new Uri(baseUri, "/api/answer");
        _preprocessQuestionsUri = new Uri(baseUri, "/api/preprocess-query");
    }

    public async Task<Vector> Embed(string query, CancellationToken cancellationToken)
    {
        Uri uri = new UriBuilder(_embedUri)
        {
            Query = $"query={Uri.EscapeDataString(query)}"
        }.Uri;

        using HttpResponseMessage response = await _httpClient.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        return new Vector(JsonSerializer.Deserialize<float[]>(content) ?? throw new InvalidOperationException());
    }

    private record AnswerPayload(int Id, string Answer, float Score);

    public async Task<List<AnswerResponse>> AnswerQuestions(string query,
        IReadOnlyCollection<SearchResultDto> searchResults,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            Query = query,
            Answers = searchResults.Select(r => new { r.Id, r.Content })
        };
        var content = JsonContent.Create(payload, options: _jsonOptions);
        // StringContent content = new StringContent(payload, new MediaTypeHeaderValue("application/json"));
        using HttpResponseMessage response =
            await _httpClient.PostAsync(_answerQuestionsUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        List<AnswerPayload> answers =
            JsonSerializer.Deserialize<List<AnswerPayload>>(responseContent, options: _jsonOptions)
            ?? throw new InvalidOperationException();
        var result = new List<AnswerResponse>();
        for (var i = 0; i < answers.Count; i++)
        {
            AnswerPayload answer = answers[i];
            SearchResultDto? searchResult = searchResults.FirstOrDefault(r => r.Id == answer.Id);
            if (searchResult is null)
            {
                _logger.LogWarning("No search result found for answer {Answer} for document {Id}", answer.Answer,
                    answer.Id);
                continue;
            }

            result.Add(new AnswerResponse
            {
                Id = i + 1,
                ReferenceCode = searchResult.ReferenceCode,
                Snippet = searchResult.Snippet,
                Content = searchResult.Content,
                Answer = answer.Answer,
                Score = answer.Score,
                Uri = searchResult.Uri,
            });
        }

        return result;
    }

    private class PreprocessPayload
    {
        [JsonPropertyName("is_question")] public required bool IsQuestion { get; set; }
        [JsonPropertyName("normalized_query")] public required string Query { get; set; }
    }

    public async Task<PreprocessedQueryResponse> IsQuestion(string query, CancellationToken cancellationToken)
    {
        Uri uri = new UriBuilder(_preprocessQuestionsUri)
        {
            Query = $"query={Uri.EscapeDataString(query)}"
        }.Uri;
        using HttpResponseMessage response = await _httpClient.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        PreprocessPayload payload = JsonSerializer.Deserialize<PreprocessPayload>(content, options: _jsonOptions)
                                    ?? throw new InvalidOperationException();
        return new PreprocessedQueryResponse(payload.IsQuestion, payload.Query);
    }
}