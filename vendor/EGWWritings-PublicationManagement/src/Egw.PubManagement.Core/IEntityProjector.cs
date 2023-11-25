using Egw.PubManagement.Core.GraphQl;
namespace Egw.PubManagement.Core;

/// <summary>
/// Entity projector
/// </summary>
public interface IEntityProjector<in TInput, TOutput> where TOutput : IProjectedDto<TInput>
{
    /// <summary>
    /// Projects entity to another 
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IQueryable<TOutput>> Project(IQueryable<TInput> queryable, CancellationToken cancellationToken);
}