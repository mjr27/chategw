namespace Egw.PubManagement.Application.Models.Internal;

/// <summary>
/// Link storage
/// </summary>
public class LinkStorage
{
    /// <summary>
    /// Stored language
    /// </summary>
    /// <param name="EgwCode">EGW Code</param>
    /// <param name="NormalizedCode">Normalized Code</param>
    public record StoredLanguage(string EgwCode, string NormalizedCode);

    /// <summary>
    /// List of stored languages
    /// </summary>
    public ICollection<StoredLanguage> Languages { get; init; } = new List<StoredLanguage>();
    // public ICollection<StoredLink> Links { get; set; } = new List<StoredLink>();
    /// <summary>
    /// Stored links
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> Links { get; init; } = new();

    /// <summary>
    /// Adds a language
    /// </summary>
    /// <param name="egwCode">EGW Code</param>
    /// <param name="normalizedCode">Normalized Code</param>
    public void AddLanguage(string egwCode, string normalizedCode)
    {
        Languages.Add(new StoredLanguage(egwCode, normalizedCode));
    }

    /// <summary>
    /// Adds link
    /// </summary>
    /// <param name="language">Normalized language</param>
    /// <param name="reference">Reference</param>
    /// <param name="paraId">Paragraph ID</param>
    public void AddLink(string language, string reference, string paraId)
    {
        if (!Links.TryGetValue(language, out Dictionary<string, string>? langDict))
        {
            langDict = new Dictionary<string, string>();
            Links[language] = langDict;
        }

        langDict.TryAdd(reference, paraId);
    }
}