using System.Diagnostics;
using System.Text.Json;
using ChatEgw.UI.Application.Models;
using ChatEgw.UI.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace ChatEgw.UI.Application.Impl;

internal class RawSearchEngineImpl(
    IDbContextFactory<SearchDbContext> dbContextFactory,
    ILogger<RawSearchEngineImpl> logger) : IRawSearchEngine
{
    // ReSharper disable once CognitiveComplexity
    public async Task<List<SearchResultDto>> SearchEmbeddings(Vector query,
        int limit,
        IReadOnlyCollection<PreprocessedPublicationReference> references,
        IReadOnlyCollection<PreprocessedEntity> entities,
        SearchFilterRequest filter,
        CancellationToken cancellationToken)
    {
        await using SearchDbContext db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        IQueryable<SearchChunk> filteredItems = db.Chunks;
        if (filter.Folders.Any())
        {
            filteredItems = filteredItems.Where(s =>
                s.Paragraph.Node.Children.Intersect(filter.Folders).Any());
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
        var data = new List<SearchResultDto>();
        if (entities.Any() || references.Any())
        {
            IQueryable<SearchChunk> filteredEntitiesQuery =
                await FilterEntities(filteredItems, references, entities, cancellationToken);

            if (!references.Any())
            {
                if (filter.IsEgw is not null)
                {
                    filteredEntitiesQuery =
                        filteredEntitiesQuery.Where(r => r.Paragraph.Node.IsEgw == filter.IsEgw.Value);
                }
            }

            data = await filteredEntitiesQuery
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
            logger.LogError("Found results: {Count}", data.Count);
        }

        if (!data.Any())
        {
            if (filter.IsEgw is not null)
            {
                filteredItems = filteredItems.Where(r => r.Paragraph.Node.IsEgw == filter.IsEgw.Value);
            }

            data = await filteredItems
                // ReSharper disable once EntityFramework.UnsupportedServerSideFunctionCall
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
        }

        logger.LogInformation("Search took {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        return data;
    }

    private IEnumerable<string> GetReferences(PreprocessedPublicationReference reference)
    {
        if (reference.Page is null)
        {
            yield return reference.Publication;
            yield break;
        }

        if (reference.EndPage is null)
        {
            yield return $"{reference.Publication} {reference.Page}";
        }
        else
        {
            for (int i = reference.Page.Value; i <= reference.EndPage.Value; i++)
            {
                yield return $"{reference.Publication} {i}";
            }
        }
    }

    private async Task<IQueryable<SearchChunk>> FilterEntities(
        IQueryable<SearchChunk> query,
        IReadOnlyCollection<PreprocessedPublicationReference> references,
        IEnumerable<PreprocessedEntity> entities,
        CancellationToken cancellationToken)
    {
        IDictionary<SearchEntityTypeEnum, HashSet<string>> data = await GetFilteredData(cancellationToken);
        var referenceSet = new HashSet<string>();
        foreach (PreprocessedPublicationReference reference in references)
        {
            Console.WriteLine(JsonSerializer.Serialize(reference));
            referenceSet.UnionWith(GetReferences(reference));
        }

        
        foreach (PreprocessedEntity entity in entities)
        {
            if (!data.TryGetValue(entity.Type, out HashSet<string>? values))
            {
                continue;
            }
        
            if (values.Contains(entity.Text))
            {
                query = query.Where(r => r.Entities.Any(p => p.Content == entity.Text));
            }
        }

        if (referenceSet.Any())
        {
            query = query.Where(p => p.Paragraph.References.Any(r => referenceSet.Contains(r.ReferenceCode)));
        }

        return query;
    }

    private static readonly SemaphoreSlim Sem = new(1, 1);
    private IDictionary<SearchEntityTypeEnum, HashSet<string>>? _entities;

    private async Task<IDictionary<SearchEntityTypeEnum, HashSet<string>>> GetFilteredData(
        CancellationToken cancellationToken)
    {
        if (_entities is not null)
        {
            return _entities;
        }

        await Sem.WaitAsync(cancellationToken);
        try
        {
            if (_entities is not null)
            {
                return _entities;
            }

            await using SearchDbContext db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            var entities = await db.Entities.Select(r => new { r.Type, r.Content }).ToListAsync(cancellationToken);
            _entities = entities.GroupBy(r => r.Type)
                .ToDictionary(r => r.Key, r => new HashSet<string>(r.Select(b => b.Content)));
            logger.LogInformation("Fetched {Count} entities", _entities.Sum(r => r.Value.Count));
            return _entities;
        }
        finally
        {
            Sem.Release();
        }
    }

    public async Task<List<SearchResultDto>> SearchFts(string query, int limit, SearchFilterRequest filter,
        CancellationToken cancellationToken)
    {
        await using SearchDbContext db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
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
        logger.LogInformation("Search took {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
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