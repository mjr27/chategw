using System.Diagnostics.CodeAnalysis;

using Egw.PubManagement.Core.Problems;
namespace Egw.PubManagement.Preview.Services;

/// <summary>
/// Accessible publications reader
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
public class AccessiblePublicationsService
{
    /// <summary>
    /// Retrieve accessible publications
    /// </summary>
    /// <returns></returns>
    public Task<int[]> GetAccessiblePublications(CancellationToken cancellationToken)
    {
        return Task.FromResult(new[] { 1, 2, 6, 7, 127, 128 });
    }

    /// <summary>
    /// Ensure publication is accessible
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    public async Task EnsureAccessible(int id, CancellationToken cancellationToken)
    {
        int[] accessible = await GetAccessiblePublications(cancellationToken);
        if (!accessible.Contains(id))
        {
            throw new NotAuthorizedProblemDetailsException();
        }
    }
}