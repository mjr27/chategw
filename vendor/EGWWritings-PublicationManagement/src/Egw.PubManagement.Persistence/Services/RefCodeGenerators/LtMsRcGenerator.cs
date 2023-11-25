using System.Text;

using Egw.PubManagement.Persistence.Entities;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Persistence.Services.RefCodeGenerators;

internal class LtMsRcGenerator : IRefCodeGenerator
{
    public string? Short(PublicationType _, string code, ParagraphMetadata? metadata)
    {
        if (metadata?.Pagination is null)
        {
            return null;
        }

        // M0001_1844
        var sb = new StringBuilder();
        sb.Append(code);
        sb.Append(", ");
        sb.Append(MakeLtMsPart(metadata.Pagination.Section));
        if (metadata.Pagination.Paragraph > 0)
        {
            sb.Append(", p. ");
            sb.Append(metadata.Pagination.Paragraph);
        }

        return sb.ToString();
    }

    public string? Long(PublicationType _, string title, ParagraphMetadata? metadata)
    {
        // Letters and Manuscripts — Volume 1 (1844 - 1868), Lt 3, 1847, par. 1
        if (metadata?.Pagination is null)
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.Append(title);
        sb.Append(", ");
        sb.Append(MakeLtMsPart(metadata.Pagination.Section));
        if (metadata.Pagination.Paragraph > 0)
        {
            sb.Append(", par. ");
            sb.Append(metadata.Pagination.Paragraph);
        }

        return sb.ToString();
    }

    private string MakeLtMsPart(string section)
    {
        var sb = new StringBuilder();
        sb.Append(section[0] == 'M' ? "Ms " : "Lt ");
        sb.Append(int.Parse(section.Substring(1, 4)));
        if (section[5] != '_')
        {
            sb.Append(section[5]);
        }

        sb.Append(", ");
        sb.Append(section[6..]);
        return sb.ToString();
    }
}