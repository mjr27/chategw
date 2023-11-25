using System.Text.Json;

using Egw.PubManagement.Application.Models.Internal;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.LegacyImport.LinkRepository;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Enums;

namespace Egw.PubManagement.Application.Services;

/// <summary>
/// Database json link repository
/// </summary>
public class DatabaseJsonLinkRepository : ILinkRepository
{
    private readonly ICollection<LinkStorage.StoredLanguage> _languages;
    private readonly Dictionary<string, Dictionary<string, string>> _codes;

    /// <summary>
    /// Database json link repository
    /// </summary>
    /// <param name="db"></param>
    public DatabaseJsonLinkRepository(PublicationDbContext db)
    {
        using JsonDocument? root = db.Configuration
            .Where(r => r.Key == GlobalOptionTypeEnum.LinkCache)
            .Select(r => r.Value)
            .FirstOrDefault();
        LinkStorage data = root?.Deserialize<LinkStorage>()
                           ?? throw new NotFoundProblemDetailsException("Cannot find configuration");
        _codes = data.Links;
        _languages = data.Languages;
    }

    /// <inheritdoc />
    public string? NormalizeLanguage(string lang)
    {
        if (lang == "grc")
        {
            lang = "gre";
        }

        string langLower = lang.ToLowerInvariant();
        foreach (LinkStorage.StoredLanguage language in _languages)
        {
            if (language.EgwCode == langLower || language.NormalizedCode == langLower)
            {
                return language.NormalizedCode;
            }
        }

        return null;
    }

    /// <inheritdoc />
    public string? FindParagraph(string link, string lang)
    {
        link = link.ToLowerInvariant();
        lang = lang.ToLowerInvariant();
        return _codes.TryGetValue(lang, out Dictionary<string, string>? codes)
               && codes.TryGetValue(link, out string? result)
            ? result
            : null;
    }
}