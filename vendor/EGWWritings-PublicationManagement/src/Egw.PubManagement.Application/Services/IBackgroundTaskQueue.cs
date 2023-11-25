using Egw.PubManagement.Application.Services.Background;

using Microsoft.Extensions.Hosting;

namespace Egw.PubManagement.Application.Services;

/// <summary>
/// Background task queue
/// </summary>
public interface IBackgroundTaskQueue : IHostedService
{
    /// <summary>
    /// Task history
    /// </summary>
    IEnumerable<BackgroundTask> History { get; }

    /// <summary>
    /// Background task queue
    /// </summary>
    /// <param name="id"></param>
    /// <param name="title"></param>
    /// <param name="handler"></param>
    void AddTask(Guid id, string title, Func<IServiceProvider, CancellationToken, Task> handler);
}