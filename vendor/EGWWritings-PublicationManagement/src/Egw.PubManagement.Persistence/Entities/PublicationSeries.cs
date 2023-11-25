using Egw.PubManagement.Persistence.Enums;
namespace Egw.PubManagement.Persistence.Entities;

/// <summary>
/// Publications series
/// </summary>
public class PublicationSeries : ITimeStampedEntity
{
    /// <summary>
    /// Series code
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Series title
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Series type
    /// </summary>
    public SeriesTypeEnum Type { get; private set; }

    /// <summary>
    /// List of publications in series
    /// </summary>
    public int[] Publications { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new series
    /// </summary>
    /// <param name="code"></param>
    /// <param name="title"></param>
    /// <param name="type"></param>
    /// <param name="publications"></param>
    /// <param name="createdAt"></param>
    public PublicationSeries(
        string code,
        string title,
        SeriesTypeEnum type,
        DateTimeOffset createdAt,
        params int[] publications)
    {
        Code = code;
        Title = title;
        Type = type;
        Publications = publications;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }
}