using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using Egw.PubManagement.Persistence.Enums;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Models.Internal;

/// <summary>
/// Mp3 file data object
/// </summary>
public readonly struct Mp3FileName
{
    /// <summary>
    /// Path to mp3 file (possible with PublicationId)
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Mp3 file name
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// File number
    /// </summary>
    public string Number { get; }

    /// <summary>
    /// Language
    /// </summary>
    public string Lang { get; }

    /// <summary>
    /// Voice type (m/f)
    /// </summary>
    public VoiceTypeEnum Voice { get; }

    /// <summary>
    /// Chapter ParaId
    /// </summary>
    public ParaId ParaId { get; }

    /// <summary>
    /// Mp3 file data object
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="number"></param>
    /// <param name="lang"></param>
    /// <param name="voice"></param>
    /// <param name="paraId"></param>
    public Mp3FileName(string path, string fileName, string number, string lang, VoiceTypeEnum voice, ParaId paraId)
    {
        Path = path;
        FileName = fileName;
        Number = number;
        Lang = lang;
        Voice = voice;
        ParaId = paraId;
    }

    /// <summary>
    /// Parse mp3 file name
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Mp3FileName Parse(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ValidationException("Mp3fileName Path can't be null");
        }

        if (CheckFileName(path, out Mp3FileName? obj))
        {
            return obj ?? throw new ValidationException($"Incorrect mp3 file name template format: {path}");
        }

        throw new ArgumentException($"Incorrect file name. {path}");
    }

    /// <summary>
    /// Parse mp3 file name
    /// </summary>
    /// <param name="path"></param>
    /// <param name="mp3"></param>
    /// <returns></returns>
    public static bool TryParse(string? path, out Mp3FileName mp3)
    {
        mp3 = default;
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        if (!CheckFileName(path, out Mp3FileName? obj))
        {
            return false;
        }

        if (obj is null)
        {
            return false;
        }

        mp3 = (Mp3FileName)obj;
        return true;
    }

    private static bool CheckFileName(string path, out Mp3FileName? obj)
    {
        Regex reMp3FileTemplate = new(
            @"^(?<fileNumber>[0-9]{4})_(?<lang>[a-z]{3})_(?<voice>[fm])_(?<name>[a-z0-9_]+)_(?<publicationId>[0-9]+)_(?<paragraphId>[0-9]+)\.mp3$",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        obj = null;
        string fileName = System.IO.Path.GetFileName(path);
        if (string.IsNullOrEmpty(fileName))
        {
            return false;
        }

        Match m = reMp3FileTemplate.Match(fileName);
        if (m.Success)
        {
            obj = new Mp3FileName(
                path,
                fileName,
                m.Groups["fileNumber"].Value,
                m.Groups["lang"].Value,
                m.Groups["voice"].Value == "m" ? VoiceTypeEnum.Male : VoiceTypeEnum.Female,
                new ParaId(int.Parse(m.Groups["publicationId"].Value),
                    int.Parse(m.Groups["paragraphId"].Value))
            );
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(this.Path);

    /// <inheritdoc />
    public override string ToString()
    {
        return this.FileName;
    }

    /// <summary>Equality operator</summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns></returns>
    public static bool operator ==(Mp3FileName left, Mp3FileName right) => left.FileName == right.FileName;

    /// <summary>Inequality operator</summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns></returns>
    public static bool operator !=(Mp3FileName left, Mp3FileName right) => !(left == right);

    /// <summary>Equality check</summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Mp3FileName other) => this.Path == other.Path && this.FileName == other.FileName;

    /// <summary>Equality check</summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => obj is Mp3FileName other && this.Equals(other);
}