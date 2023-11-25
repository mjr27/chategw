namespace Egw.PubManagement.Application.Messaging.Files;

/// <summary> Export epub </summary>
public class ExportEpubInput : IApplicationCommand
{
    /// <summary> Publication id </summary>
    public required int PublicationId { get; init; }
}