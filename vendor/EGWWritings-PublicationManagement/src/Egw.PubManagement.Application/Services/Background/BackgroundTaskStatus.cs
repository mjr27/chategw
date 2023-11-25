namespace Egw.PubManagement.Application.Services.Background;

/// <summary>
/// Background task status
/// </summary>
public enum BackgroundTaskStatus
{
    /// <summary>
    /// Task is
    /// </summary>
    Pending,

    /// <summary>
    /// Task is started
    /// </summary>
    Started,

    /// <summary>
    /// Task is completed successfully
    /// </summary>
    Completed,

    /// <summary>
    /// Task is failed
    /// </summary>
    Failed
}