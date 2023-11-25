namespace Egw.PubManagement.Application.Services.Background;

/// <summary>
/// Background task
/// </summary>
public abstract class BackgroundTask
{
    /// <summary>
    /// Task ID
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Task title
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Task status
    /// </summary>
    /// 
    public abstract BackgroundTaskStatus Status { get; }

    /// <summary>
    /// Task start date
    /// </summary>
    public DateTimeOffset? StartedAt { get; protected set; }

    /// <summary>
    /// Task end date
    /// </summary>
    public DateTimeOffset? CompletedAt { get; protected set; }


    /// <summary>
    /// Error message
    /// </summary>
    public abstract string? ErrorMessage { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public BackgroundTask(Guid id, string title)
    {
        Id = id;
        Title = title;
    }
}