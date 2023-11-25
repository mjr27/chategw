using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
namespace Egw.PubManagement.LegacyImport.PublicationTypeEnrichers;

internal static class WemlParagraphExtensions
{
    public static bool IsHeaderOfLevel(this WemlParagraph paragraph, int level)
    {
        return paragraph.Element is WemlHeadingContainer text && text.Level == level;
    }
}