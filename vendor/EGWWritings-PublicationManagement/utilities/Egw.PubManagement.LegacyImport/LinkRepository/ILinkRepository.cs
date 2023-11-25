namespace Egw.PubManagement.LegacyImport.LinkRepository;

/// <summary>
/// Link repository
/// </summary>
public interface ILinkRepository
{
    /// <summary>
    /// Normalizes language code
    /// </summary>
    /// <param name="lang">source language</param>
    /// <returns>Normalized language code or null</returns>
    string? NormalizeLanguage(string lang);

    /// <summary>
    /// Finds paragraph
    /// </summary>
    /// <param name="link">Link text</param>
    /// <param name="lang">Language code</param>
    /// <returns></returns>
    string? FindParagraph(string link, string lang);
}