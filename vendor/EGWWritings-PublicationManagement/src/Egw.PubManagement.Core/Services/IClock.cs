namespace Egw.PubManagement.Core.Services;

/// <summary>
/// Clock interface
/// </summary>
public interface IClock
{
    /// <summary>
    /// Current time
    /// </summary>
    DateTimeOffset Now { get; }
}