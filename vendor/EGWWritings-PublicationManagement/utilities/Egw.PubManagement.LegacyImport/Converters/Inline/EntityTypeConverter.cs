using System.Globalization;

using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class EntityTypeConverter : ILegacyInlineConverter
{
    private static readonly Dictionary<string, WemlEntityType> Entities = new()
    {
        ["ltaddressee"] = WemlEntityType.Addressee,
        ["ltplace"] = WemlEntityType.Place,
        ["msplace"] = WemlEntityType.Place,
        ["ltdate"] = WemlEntityType.Date,
        ["msdate"] = WemlEntityType.Date
    };

    private static readonly Dictionary<string, Func<IElement, string?>> ValueFetchers = new()
    {
        ["ltaddressee"] = FetchValue, ["ltplace"] = FetchValue, ["ltdate"] = FetchStartDate, ["msdate"] = FetchStartDate
    };

    private static string? FetchValue(IElement node)
    {
        return node.GetAttribute("value");
    }

    private static string? FetchStartDate(IElement node)
    {
        string? start = node.GetAttribute("start");
        if (start is null || start.ToUpperInvariant() == "ND")
        {
            return null;
        }

        if (DateOnly.TryParse(start, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateOnly date))
        {
            return date.ToString("O");
        }

        if (int.TryParse(start, out int year))
        {
            return new DateOnly(year, 1, 1).ToString("O");
        }
        Console.WriteLine($"Invalid date: {start}");
        return null;
    }

    public bool CanProcess(INode node) => node is IElement
                                          {
                                              NodeName: "SPAN",
                                          } el
                                          && Entities.ContainsKey(el.ClassName ?? "");

    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node;
        return new WemlEntityElement(
            Entities[element.ClassName ?? ""],
            parser.ParseChildInlines(node, context)
        ) { Value = ValueFetchers.GetValueOrDefault(element.ClassName ?? "")?.Invoke(element) };
    }
}