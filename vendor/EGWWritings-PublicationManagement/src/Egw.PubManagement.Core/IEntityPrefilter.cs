namespace Egw.PubManagement.Core;

/// <summary>
/// Entity projector
/// </summary>
public interface IEntityPrefilter<TInput>
{
    /// <summary>
    /// Projects entity to another 
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IQueryable<TInput>> Filter(IQueryable<TInput> queryable, CancellationToken cancellationToken);
}