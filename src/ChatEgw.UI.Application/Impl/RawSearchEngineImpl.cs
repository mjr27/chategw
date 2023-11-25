using System.Diagnostics;
using ChatEgw.UI.Application.Models;
using ChatEgw.UI.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace ChatEgw.UI.Application.Impl;

internal class RawSearchEngineImpl : IRawSearchEngine
{
    private readonly IDbContextFactory<SearchDbContext> _dbContextFactory;
    private readonly ILogger<RawSearchEngineImpl> _logger;

    public RawSearchEngineImpl(
        IDbContextFactory<SearchDbContext> dbContextFactory,
        ILogger<RawSearchEngineImpl> logger
    )
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<List<SearchResultDto>> SearchEmbeddings(Vector query,
        int limit,
        SearchFilterRequest filter,
        CancellationToken cancellationToken)
    {
        await using SearchDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        IQueryable<SearchChunk> filteredItems = db.Chunks;
        if (filter.Folders.Any())
        {
            filteredItems = filteredItems.Where(s =>
                EF.Functions.JsonExistAny(s.Paragraph.Node.Children, filter.Folders));
        }

        if (filter.IsEgw is not null)
        {
            filteredItems = filteredItems.Where(r => r.Paragraph.Node.IsEgw == filter.IsEgw.Value);
        }

        if (filter.MinDate is not null)
        {
            filteredItems = filteredItems.Where(r => r.Paragraph.Node.Date >= filter.MinDate.Value);
        }

        if (filter.MaxDate is not null)
        {
            filteredItems = filteredItems.Where(r => r.Paragraph.Node.Date <= filter.MaxDate.Value);
        }

        await using IDbContextTransaction t = await db.Database.BeginTransactionAsync(cancellationToken);
        var sw = Stopwatch.StartNew();
        await ConfigureIndex(db, cancellationToken);
        List<SearchResultDto> data = await filteredItems
            .OrderBy(r => r.Embedding.MaxInnerProduct(query))
            .Take(limit)
            .Select(r => new SearchResultDto
            {
                Id = r.Id,
                ReferenceCode = r.Paragraph.RefCode,
                Snippet = r.Content,
                Content = r.Paragraph.Content,
                Uri = r.Paragraph.Uri,
                Distance = r.Embedding.MaxInnerProduct(query)
            })
            .ToListAsync(cancellationToken: cancellationToken);
        _logger.LogInformation("Search took {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        return data;
    }

    public async Task<List<SearchResultDto>> SearchFts(string query, int limit, SearchFilterRequest filter,
        CancellationToken cancellationToken)
    {
        await using SearchDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var sw = Stopwatch.StartNew();
        IQueryable<SearchParagraph> filteredItems = db.Paragraphs;
        if (filter.Folders.Any())
        {
            filteredItems = filteredItems.Where(s =>
                EF.Functions.JsonExistAny(s.Node.Children, filter.Folders));
        }

        if (filter.IsEgw is not null)
        {
            filteredItems = filteredItems.Where(r => r.Node.IsEgw == filter.IsEgw.Value);
        }

        if (filter.MinDate is not null)
        {
            filteredItems = filteredItems.Where(r => r.Node.Date >= filter.MinDate.Value);
        }

        if (filter.MaxDate is not null)
        {
            filteredItems = filteredItems.Where(r => r.Node.Date <= filter.MaxDate.Value);
        }

        List<SearchResultDto> result = await filteredItems
            .Where(r => EF.Functions.ToTsVector("english", r.Content).Matches(query))
            .Take(limit)
            .Select(r => new SearchResultDto
            {
                Id = r.Id,
                ReferenceCode = r.RefCode,
                Snippet = EF.Functions.PlainToTsQuery(query).GetResultHeadline(r.Content,
                    "MinWords=10,MaxWords=20,HighlightAll=true,MaxFragments=5,StartSel=###BEGIN###,StopSel=###END###"),
                Content = r.Content,
                Uri = r.Uri,
                Distance = EF.Functions.ToTsVector("english", r.Content).Rank(EF.Functions.PlainToTsQuery(query))
            })
            .OrderByDescending(r => r.Distance)
            .ToListAsync(cancellationToken: cancellationToken);
        _logger.LogInformation("Search took {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        return result;
    }

    private static async Task ConfigureIndex(DbContext db, CancellationToken cancellationToken)
    {
        const long ivFlatProbes = 100;
        const long efSearch = 100;
        var sql = $"SET LOCAL ivfflat.probes = {ivFlatProbes}";
        await db.Database.ExecuteSqlRawAsync(sql, cancellationToken: cancellationToken);
        sql = $"SET LOCAL hnsw.ef_search = {efSearch}";
        await db.Database.ExecuteSqlRawAsync(sql, cancellationToken: cancellationToken);
    }
}