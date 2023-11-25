using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Application.Services.Background;

namespace Egw.PubManagement.Services;

/// <inheritdoc cref="IBackgroundTaskQueue" />
public class BackgroundTaskQueueImpl : BackgroundService, IBackgroundTaskQueue
{
    private readonly IServiceProvider _serviceProvider;

    private class BackgroundTaskImpl : BackgroundTask
    {
        private readonly Func<IServiceProvider, CancellationToken, Task> _handler;
        private Exception? _exception;

        public async Task Execute(IServiceProvider db, CancellationToken cancellationToken)
        {
            StartedAt = DateTimeOffset.UtcNow;
            try
            {
                await _handler.Invoke(db, cancellationToken);
            }
            catch (Exception e)
            {
                _exception = e;
            }
            finally
            {
                CompletedAt = DateTimeOffset.UtcNow;
            }
        }

        public BackgroundTaskImpl(Guid id, string title, Func<IServiceProvider, CancellationToken, Task> handler) :
            base(id, title)
        {
            _handler = handler;
        }

        public override BackgroundTaskStatus Status => StartedAt is null ? BackgroundTaskStatus.Pending
            : CompletedAt is null ? BackgroundTaskStatus.Started
            : _exception is null ? BackgroundTaskStatus.Completed
            : BackgroundTaskStatus.Failed;

        public override string? ErrorMessage => _exception?.Message;
    }

    private readonly LinkedList<BackgroundTaskImpl> _history = new();
    private readonly Queue<BackgroundTaskImpl> _queue = new(100);


    /// <inheritdoc />
    public IEnumerable<BackgroundTask> History => _history.Concat(_queue);

    /// <inheritdoc />
    public void AddTask(Guid id, string title, Func<IServiceProvider, CancellationToken, Task> handler)
    {
        _queue.Enqueue(new BackgroundTaskImpl(id, title, handler));
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            if (!_queue.TryDequeue(out BackgroundTaskImpl? task))
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            _history.AddLast(task);
            while (_history.Count > HistorySize)
            {
                _history.RemoveFirst();
            }

            await task.Execute(_serviceProvider, stoppingToken);
        }
    }

    /// <summary>
    /// History size
    /// </summary>
    public const int HistorySize = 100;

    /// <summary>
    /// Default constructor
    /// </summary>
    public BackgroundTaskQueueImpl(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
}