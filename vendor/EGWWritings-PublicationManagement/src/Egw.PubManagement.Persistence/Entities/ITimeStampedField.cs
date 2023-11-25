namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Timestamp tracked field
/// </summary>
public interface ITimeStampedEntity
{
    /// <summary>
    /// Created at
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Updated at
    /// </summary>
    DateTimeOffset UpdatedAt { get; }
}