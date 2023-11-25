using MediatR;
namespace Egw.ImportUtils.Messaging.Commands;

/// <summary>
/// Export old publications to target directory
/// </summary>
/// <param name="ConnectionString">Connection string</param>
/// <param name="OutputFolder">Output folder</param>
public record ExportMultiplePublicationsFromEgwOldCommand(string ConnectionString, string OutputFolder) : IRequest
{
    /// <summary>
    /// Skips existing publication
    /// </summary>
    public bool SkipExisting { get; set; }

    /// <summary>
    /// Min publication id
    /// </summary>
    public int Min { get; set; }

    /// <summary>
    /// Max publication id
    /// </summary>
    public int Max { get; set; }
}