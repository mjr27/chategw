using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport;

/// <inheritdoc />
public class LegacyPublicationTypeDetector : ILegacyPublicationTypeDetector
{
    /// <inheritdoc />
    public PublicationType? GetPublicationType(string type, string? subType)
    {
        subType = subType?.ToLowerInvariant().Trim() ?? "";
        return type.ToLowerInvariant() switch
        {
            "book" when subType is "devotional" => PublicationType.Devotional,
            "book" when subType is "commentary" => PublicationType.BibleCommentary,
            "book" when subType is "modernenglish" => PublicationType.Book,
            "book" when subType is "book" => PublicationType.Book,
            "book" when string.IsNullOrWhiteSpace(subType) => PublicationType.Book,
            "letter" or "sermon" or "study guide" => PublicationType.Book, // WTF?$
            "bible" => PublicationType.Bible,
            "scriptindex" => PublicationType.ScriptureIndex,
            "topicalindex" when string.IsNullOrEmpty(subType) => PublicationType.TopicalIndex,
            "dictionary" when string.IsNullOrEmpty(subType) => PublicationType.Dictionary,
            "ltms" => PublicationType.Manuscript,
            "periodical" when subType is "no pagebreaks" => PublicationType.PeriodicalNoPageBreak,
            "periodical" when subType is "" => PublicationType.PeriodicalPageBreak,
            _ => null
        };
    }
}