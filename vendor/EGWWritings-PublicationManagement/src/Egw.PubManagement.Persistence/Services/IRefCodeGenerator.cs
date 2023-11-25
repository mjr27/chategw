using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Persistence.Services;

/// <summary>
/// Reference code generator
/// </summary>
public interface IRefCodeGenerator
{
    /// <summary>
    /// Reference code generator static instance
    /// </summary>
    public static IRefCodeGenerator Instance { get; } = new RefCodeGenerator();

    /// <summary>
    /// Generates short code for paragraph
    /// </summary>
    /// <param name="type">Publication type</param>
    /// <param name="code">Publication code</param>
    /// <param name="metadata">Paragraph metadata</param>
    /// <returns></returns>
    string? Short(PublicationType type, string code, ParagraphMetadata? metadata);

    /// <summary>
    /// Generates long code for paragraph
    /// </summary>
    /// <param name="type">Publication type</param>
    /// <param name="title">Publication title</param>
    /// <param name="metadata">Paragraph metadata</param>
    /// <returns></returns>
    string? Long(PublicationType type, string title, ParagraphMetadata? metadata);
}