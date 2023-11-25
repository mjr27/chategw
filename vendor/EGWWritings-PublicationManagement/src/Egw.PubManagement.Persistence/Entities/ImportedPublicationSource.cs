namespace Egw.PubManagement.Persistence.Entities;

/// <summary> Publication source </summary>
public class ImportedPublicationSource
{
    /// <summary> Publication id </summary>
    public required int PublicationId { get; init; }

    /// <summary> Source </summary>
    public required string Source { get; init; } = null!;

    /// <summary> Source Identifier </summary>
    public required string SourceIdentifier { get; init; }

    /// <summary> Import date </summary>
    public required DateTimeOffset ImportDate { get; init; }
}