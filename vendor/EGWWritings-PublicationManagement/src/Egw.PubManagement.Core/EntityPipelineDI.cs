using Egw.PubManagement.Core.GraphQl;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Core;

/// <summary>
/// Entity pipeline extensions
/// </summary>
public static class EntityPipelineDI
{
    /// <summary>
    /// Applies query pipeline
    /// </summary>
    /// <param name="queryable">Queryable</param>
    /// <param name="filter">Filter pipeline</param>
    /// <param name="projector">Projection pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="TInput">Input type</typeparam>
    /// <typeparam name="TOutput">Output type</typeparam>
    /// <returns></returns>
    public static async Task<IQueryable<TOutput>> ApplyPipeline<TInput, TOutput>(
        this IQueryable<TInput> queryable,
        IEntityPrefilter<TInput> filter,
        IEntityProjector<TInput, TOutput> projector,
        CancellationToken cancellationToken) where TOutput : IProjectedDto<TInput>
    {
        IQueryable<TInput> filteredData = await filter.Filter(queryable, cancellationToken);
        return await projector.Project(filteredData, cancellationToken);
    }

    /// <summary>
    /// To data loader result
    /// </summary>
    /// <param name="results"></param>
    /// <param name="keys"></param>
    /// <param name="keySelector"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static async Task<Dictionary<TKey, TValue?>> ToLoaderResult<TValue, TKey>(
        this IQueryable<TValue> results,
        IEnumerable<TKey> keys,
        Func<TValue, TKey> keySelector,
        CancellationToken cancellationToken) where TValue : class where TKey : notnull
    {
        var result = keys.ToDictionary(r => r, _ => (TValue?)null);
        List<TValue> queryResults = await results.ToListAsync(cancellationToken);
        foreach (TValue item in queryResults)
        {
            TKey key = keySelector(item);
            result[key] = item;
        }

        return result;
    }

    /// <summary>
    /// To data loader result
    /// </summary>
    /// <param name="results"></param>
    /// <param name="keys"></param>
    /// <param name="keySelector"></param>
    /// <param name="valueSelector">Value selector</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<Dictionary<TKey, TValue?>> ToLoaderResult<TValue, TKey, TResult>(
        this IQueryable<TResult> results,
        IEnumerable<TKey> keys,
        Func<TResult, TKey> keySelector,
        Func<TResult, TValue> valueSelector,
        CancellationToken cancellationToken) where TResult : class where TKey : notnull
    {
        var result = keys.ToDictionary(r => r, _ => default(TValue));
        List<TResult> queryResults = await results.ToListAsync(cancellationToken);
        foreach (TResult item in queryResults)
        {
            TKey key = keySelector(item);
            result[key] = valueSelector(item);
        }

        return result;
    }
}