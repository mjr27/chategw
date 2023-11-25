namespace Egw.PubManagement.Models;

/// <summary>
/// Queue log entry
/// </summary>
/// <param name="PublicationId">Publication id</param>
/// <param name="FileSize">File size</param>
/// <param name="ProcessedAt">Date and time of processing</param>
/// <param name="DurationMs">Duration in milliseconds</param>
/// <param name="Error">Error message</param>
public record QueueLogEntry(int PublicationId, int FileSize, DateTimeOffset ProcessedAt, int DurationMs,
    string? Error);