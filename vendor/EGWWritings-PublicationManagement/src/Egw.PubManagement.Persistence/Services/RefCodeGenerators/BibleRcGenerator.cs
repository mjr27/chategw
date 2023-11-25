using System.Text;

using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Entities.Metadata;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Persistence.Services.RefCodeGenerators;

internal class BibleRcGenerator : IRefCodeGenerator
{
    public string? Short(PublicationType _, string code, ParagraphMetadata? metadata)
    {
        if (metadata?.BibleMetadata is null)
        {
            return null;
        }

        BibleMetadata? bibleMetadata = metadata.BibleMetadata;

        var sb = new StringBuilder();
        sb.Append(code);
        sb.Append(" — ");
        sb.Append(bibleMetadata.Book);
        if (bibleMetadata.Chapter > 0)
        {
            sb.Append(' ');
            sb.Append(bibleMetadata.Chapter);
            if (bibleMetadata.Verses.Count > 0)
            {
                sb.Append(':');
                sb.Append(string.Join(',', bibleMetadata.Verses));
            }
        }

        return sb.ToString();
    }

    public string? Long(PublicationType _, string title, ParagraphMetadata? metadata)
    {
        if (metadata?.BibleMetadata is null)
        {
            return null;
        }

        BibleMetadata? bibleMetadata = metadata.BibleMetadata;

        var sb = new StringBuilder();
        sb.Append(title);
        sb.Append(" — ");
        sb.Append(bibleMetadata.Book);
        if (bibleMetadata.Chapter > 0)
        {
            sb.Append(' ');
            sb.Append(bibleMetadata.Chapter);
            if (bibleMetadata.Verses.Count > 0)
            {
                sb.Append(':');
                sb.Append(string.Join(',', bibleMetadata.Verses));
            }
        }

        return sb.ToString();
    }
}