using System.Text;

using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Persistence.Services.RefCodeGenerators;

internal class PeriodicalPageBreakRcGenerator : IRefCodeGenerator
{
    public string? Short(PublicationType type, string code, ParagraphMetadata? metadata)
    {
        if (metadata?.Date is null)
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.Append(code);
        sb.Append(' ');
        sb.Append(metadata.Date.Value.ToString("MMMM d, yyyy"));
        if (metadata.Pagination is null)
        {
            return sb.ToString();
        }

        sb.Append(", page ");
        sb.Append(metadata.Pagination.Section);
        if (metadata.Pagination.Paragraph > 0)
        {
            sb.Append('.');
            sb.Append(metadata.Pagination.Paragraph);
        }

        return sb.ToString();
    }

    public string? Long(PublicationType type, string title, ParagraphMetadata? metadata)
    {
        if (metadata?.Date is null)
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.Append(title);
        sb.Append(' ');
        sb.Append(metadata.Date.Value.ToString("MMMM d, yyyy"));
        if (metadata.Pagination is null)
        {
            return sb.ToString();
        }

        sb.Append(", page ");
        sb.Append(metadata.Pagination.Section);
        if (metadata.Pagination.Paragraph > 0)
        {
            sb.Append(", paragraph ");
            sb.Append(metadata.Pagination.Paragraph);
        }

        return sb.ToString();
    }
}