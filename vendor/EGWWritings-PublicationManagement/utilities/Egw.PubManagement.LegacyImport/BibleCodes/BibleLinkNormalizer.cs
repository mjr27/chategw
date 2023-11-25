using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
namespace Egw.PubManagement.LegacyImport.BibleCodes;

/// <summary>
/// Normalizes bible links
/// </summary>
public class BibleLinkNormalizer
{
    private class BibleBook
    {
        public string Name { get; }
        public int ChapterCount { get; }
        public List<string> Synonyms { get; }

        public BibleBook(string name, int chapterCount, IEnumerable<string> synonyms)
        {
            Name = name;
            ChapterCount = chapterCount;
            Synonyms = new List<string>();
            Synonyms.AddRange(synonyms);
        }
    }

    private static readonly Regex ReSpaces = new(@"\s+");
    private static readonly Regex MergeSpaces = new(@"^(\d+)\s+(.*)$");
    private readonly IReadOnlyCollection<(string prefix, string normalized, int chapterCount)> _books;

    /// <summary>
    /// Default constructor 
    /// </summary>
    public BibleLinkNormalizer()
    {
        XDocument document = Load("bibles.xml");
        _books = LoadBooks(document)
            .SelectMany(book =>
            {
                return book.Synonyms.Select(syn => (syn, book.Name, book.ChapterCount));
            })
            .OrderByDescending(r => r.syn.Length)
            .ToList();
    }

    /// <summary>
    /// Normalizes bible refcode/link
    /// </summary>
    /// <param name="refCode">Raw link/reference code</param>
    /// <returns></returns>
    public string NormalizeRefCode(string refCode)
    {
        string normalizedCode = NormalizeSpaces(RemovePunctuation(refCode.ToLowerInvariant().Normalize()));

        string? result = Match(normalizedCode);
        if (result is not null)
        {
            return result;
        }

        if (!char.IsDigit(normalizedCode[0]))
        {
            return normalizedCode;
        }

        string fixedCode = MergeSpaces.Replace(normalizedCode, "$1$2");
        return Match(fixedCode) ?? normalizedCode;
    }

    private string? Match(string code)
    {
        foreach ((string prefix, string normalized, int chapterCount) in _books)
        {
            if (!StringMatches(code, prefix, out string remainder)
                || (remainder.Length != 0 && char.IsLetter(remainder[0])))
            {
                continue;
            }

            if (chapterCount != 1)
            {
                return NormalizeSpaces(normalized + ' ' + remainder);
            }

            string[] remainderArray = remainder.Trim().Split(' ', 2);
            if (remainderArray.Length < 2 || remainderArray[1] != "1" || int.Parse(remainderArray[0]) <= 1)
            {
                return NormalizeSpaces(normalized + ' ' + remainder);
            }

            return NormalizeSpaces(normalized + ' ' + $" {remainderArray[1]} {remainderArray[0]}");
        }

        return null;
    }

    private static IEnumerable<BibleBook> LoadBooks(XDocument document)
    {
        XElement? root = document.Root;
        if (root == null || root.Name.ToString() != "bible")
        {
            yield break;
        }

        foreach (XElement bookElement in root.Elements().Where(node => node.Name == "book"))
        {
            BibleBook book = LoadSingleBook(bookElement);
            yield return book;
        }
    }

    private static BibleBook LoadSingleBook(XElement bookElement)
    {
        string bookName = bookElement.Attribute("name")!.Value.ToLowerInvariant();
        int chapterCount = int.Parse(bookElement.Attribute("chapters")!.Value);
        string bookCode = bookElement.Attribute("code")!.Value.ToLowerInvariant();

        var synonyms = new List<string>();
        foreach (XElement translationElement in bookElement.Elements().Where(r => r.Name == "syn"))
        {
            string translation = translationElement.Value.ToLowerInvariant();
            synonyms.Add(translation);
        }

        return new BibleBook(bookCode.Normalize(), chapterCount, synonyms.Prepend(bookName).Select(s => s.Normalize()));
    }

    private static XDocument Load(string filename)
    {
        Assembly assembly = typeof(BibleLinkNormalizer).Assembly;

        string basePath = assembly.GetManifestResourceBaseName($"Resources.{filename}");
        using Stream? stream = assembly.GetManifestResourceStream(basePath);
        if (stream == null)
        {
            return new XDocument();
        }

        using var reader = XmlReader.Create(stream);
        return XDocument.Load(reader);
    }


    private static string NormalizeSpaces(string value)
    {
        return ReSpaces.Replace(value, " ").Trim();
    }


    private static string RemovePunctuation(string value)
    {
        char[] arr = value.ToCharArray();
        for (int i = 0; i < value.Length; i++)
        {
            if (char.IsPunctuation(arr[i]))
            {
                arr[i] = ' ';
            }
        }

        return new string(arr);
    }

    private static bool StringMatches(string refCode, string prefix, out string remainder)
    {
        if (refCode.StartsWith(prefix, StringComparison.Ordinal))
        {
            remainder = refCode[prefix.Length..];
            return true;
        }

        remainder = "";
        return false;
    }
}