using System.Text;

using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Persistence.Services.RefCodeGenerators;

internal class DictionaryRcGenerator : IRefCodeGenerator
{
    public string? Short(PublicationType _, string code, ParagraphMetadata? metadata)
    {
        if (metadata?.Pagination is null)
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.Append(code);

        sb.Append(' ');
        sb.Append(metadata.Pagination.Section);
        if (metadata.Pagination.Paragraph <= 0)
        {
            return sb.ToString();
        }

        sb.Append('.');
        sb.Append(metadata.Pagination.Paragraph);

        return sb.ToString();
    }

    public string? Long(PublicationType _, string title, ParagraphMetadata? metadata)
    {
        if (metadata?.Pagination is null)
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.Append(title);

        sb.Append(", ");
        sb.Append(metadata.Pagination.Section);
        if (metadata.Pagination.Paragraph > 0)
        {
            sb.Append('.');
            sb.Append(metadata.Pagination.Paragraph);
        }

        return sb.ToString();
    }
}