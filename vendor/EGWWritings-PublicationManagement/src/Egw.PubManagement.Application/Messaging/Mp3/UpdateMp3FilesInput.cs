using System.Text.Json.Serialization;

using Egw.PubManagement.Application.Models.Internal;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Messaging.Mp3;

/// <summary>
/// Assign mp3 file input
/// </summary>
public class UpdateMp3FilesInput : IApplicationCommand
{
    /// <summary>
    /// Publication Id
    /// </summary>
    public required int PublicationId { get; init; }

    /// <summary>
    /// Mp3 files list
    /// </summary>
    public required List<AssignedMp3File> Mp3Files { get; init; }
}

/// <summary>
/// Mp3 file item
/// </summary>
public class AssignedMp3File
{
    /// <summary>
    /// Paragraph Id
    /// </summary>
    [JsonConverter(typeof(ParaIdConverter))]
    public required ParaId ParaId { get; init; }

    /// <summary>
    /// Mp3 file name
    /// </summary>
    [JsonConverter(typeof(Mp3FileNameConverter))]
    public required Mp3FileName File { get; init; }
}