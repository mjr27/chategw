using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Application.Services.Background;

namespace Egw.PubManagement.GraphQl.Namespaces;

/// <summary>
/// Queue-related queries
/// </summary>
public class QueueNamespace
{
    /// <summary>
    /// Queue size
    /// </summary>
    /// <returns></returns>
    public int QueueSize([Service] IBackgroundTaskQueue importService)
    {
        return importService.History.Count(r => r.StartedAt is null);
    }

    /// <summary>
    /// Queue
    /// </summary>
    /// <returns></returns>
    public ICollection<BackgroundTask> Queue([Service] IBackgroundTaskQueue importService)
    {
        return importService.History.Where(r => r.StartedAt is null).ToArray();
    }

    /// <summary>
    /// Completed logs
    /// </summary>
    /// <returns></returns>
    public ICollection<BackgroundTask> Log([Service] IBackgroundTaskQueue importService)
    {
        return importService.History.Where(r => r.StartedAt is not null).OrderBy(r => r.StartedAt).ToArray();
    }
}