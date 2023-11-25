using System.Diagnostics;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;

using Egw.PubManagement.LegacyImport.Internal;
using Egw.PubManagement.LegacyImport.LinkRepository;
using Egw.PubManagement.LegacyImport.PublicationTypeEnrichers;

using Microsoft.Extensions.DependencyInjection;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Serialization;
namespace Egw.PubManagement.LegacyImport;

/// <inheritdoc />
public class LegacyHtmlConverter : ILegacyHtmlConverter
{
    private readonly ILegacyElementParser _elementParser;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="linkRepository">Link repository</param>
    public LegacyHtmlConverter(ILinkRepository linkRepository)
    {
        // _context = BrowsingContext.New();
        IServiceCollection services = new ServiceCollection();

        services.AddLegacyImports();
        services.AddSingleton(linkRepository);
        _sp = services.BuildServiceProvider();
        _elementParser = _sp.GetRequiredService<ILegacyElementParser>();
        _pubTypeDetector = new LegacyPublicationTypeDetector();
    }


    private static readonly string[] SkippedClasses = { "countpara-no", "countchapter-no", "countpara-sameasprevious" };

    /// <inheritdoc />
    public IHtmlDocument Convert(IHtmlDocument document, out IReadOnlyCollection<CollectedWarning> warnings)
    {
        Debug.Assert(document.Body is not null);

        // removing comments
        foreach (IComment? comment in document.Descendents<IComment>())
        {
            comment.Remove();
        }

        foreach (IElement? item in document.QuerySelectorAll(".toc").ToArray())
        {
            item.Remove();
        }

        WemlDocumentHeader header = ExtractHeader(document);

        IEnumerable<IReadOnlyCollection<IElement>> groups = GroupElements(document.Body.Children);
        var nodes = new List<WemlParagraph>();
        var serializer = new WemlSerializer();
        serializer.EnrichWithLegacy();

        var context = new LegacyContext(header, document);

        foreach (IEnumerable<IElement> combinedElementList in groups.Select(element => Combine(document, element)))
        {
            IElement[] elements = combinedElementList.ToArray();
            IEnumerable<WemlParagraph> paragraphNodes = BuildNodes(elements, context);
            var lst = new List<WemlParagraph>();
            foreach (WemlParagraph n in paragraphNodes)
            {
                if (n.Element is not WemlPageBreakElement)
                {
                    lst.Add(n);
                    continue;
                }

                AddIfNotNull(nodes, GetCombinedNode(lst, elements.First(), context));
                lst.Clear();
                nodes.Add(n);
            }

            AddIfNotNull(nodes, GetCombinedNode(lst, elements.First(), context));
        }

        var wemlDocument = new WemlDocument(context.Header);
        foreach (WemlParagraph node in nodes)
        {
            wemlDocument.Children.Add(node);
        }

        FixParagraphNumbering(wemlDocument, context);
        warnings = context.Serialize(WarningLevel.Warning).ToList();
        IHtmlDocument newDocument = serializer.Serialize(wemlDocument);
        FixNodes(newDocument.Body!);
        return newDocument;
    }

    private static void AddIfNotNull<T>(ICollection<T> collection, T? item) where T : class
    {
        if (item != null)
        {
            collection.Add(item);
        }
    }

    private IEnumerable<WemlParagraph> BuildNodes(IElement[] elements, LegacyContext context)
    {
        var result = new List<WemlParagraph>();
        foreach (IElement combinedElement in elements)
        {
            bool isSkipped = combinedElement.ClassList.Any(SkippedClasses.Contains)
                             || !combinedElement.ClassList.Any(s => s.StartsWith("count"));

            RemoveSystemClasses(combinedElement);

            context.SetContext(combinedElement);

            IWemlContainerElement? parsedNode = _elementParser.ParseContainer(combinedElement, context);
            if (parsedNode is null)
            {
                context.Report(WarningLevel.Error, combinedElement, "Invalid node");
                continue;
            }

            SetSkipped(parsedNode, isSkipped);

            if (!int.TryParse(combinedElement.GetAttribute("id")?.Split('.').Last() ?? "", out int elementId))
            {
                context.Report(WarningLevel.Error, combinedElement, "Invalid element ID");
            }

            result.Add(new WemlParagraph(elementId, parsedNode));
        }

        return result;
    }

    private WemlParagraph? GetCombinedNode(IReadOnlyCollection<WemlParagraph> paragraphs, INode node, LegacyContext context)
    {
        if (paragraphs.Count == 0)
        {
            return null;
        }

        if (paragraphs.Count == 1)
        {
            return paragraphs.Single();
        }

        if (!paragraphs.All(p => p.Element is WemlParagraphContainer))
        {
            context.Report(WarningLevel.Error, node, "Cannot join containers of different types");
            return null;
        }

        WemlParagraph firstPara = paragraphs.First();
        var firstContainer = (WemlParagraphContainer)firstPara.Element;
        return new WemlParagraph(
            paragraphs.First().Id,
            new WemlParagraphGroupContainer(
                paragraphs.Select(r => (WemlParagraphContainer)r.Element)
            ) { Skip = firstContainer.Skip }
        );
    }


    private void SetSkipped(IWemlContainerElement node, bool isSkipped)
    {
        switch (node)
        {
            case WemlParagraphContainer pc:
                pc.Skip = pc.Skip || isSkipped;
                break;
            case WemlHeadingContainer hc:
                hc.Skip = hc.Skip || isSkipped;
                break;
        }
    }

    private void FixParagraphNumbering(WemlDocument wemlDocument, LegacyContext context)
    {
        switch (wemlDocument.Header.Type)
        {
            case PublicationType.Dictionary:
                {
                    var enricher = new DictionaryEnricher();
                    enricher.EnrichDocument(wemlDocument, context);
                    break;
                }

            case PublicationType.TopicalIndex:
                {
                    var enricher = new TopicalIndexEnricher();
                    enricher.EnrichDocument(wemlDocument, context);
                    break;
                }
            case PublicationType.PeriodicalPageBreak or PublicationType.PeriodicalNoPageBreak:
                {
                    var enricher = new PeriodicalEnricher();
                    enricher.EnrichDocument(wemlDocument, context);
                    break;
                }
        }
    }


    private WemlDocumentHeader ExtractHeader(IHtmlDocument document)
    {
        if (document.QuerySelector("table.pubinfotable") is not IHtmlTableElement table)
        {
            throw new FormatException("Document has no pubinfotable element");
        }

        table.Remove();
        var publicationInfo = table.Rows
            .ToDictionary(
                r => r.Cells.First().TextContent.ToString(),
                r => r.Cells.Last()
            );
        return new WemlDocumentHeader(
            int.Parse(publicationInfo["pubnr"].TextContent),
            GetPublicationType(
                publicationInfo["pubtype"].TextContent.ToLowerInvariant().Trim(),
                publicationInfo["subtype"].TextContent.ToLowerInvariant().Trim()
            ),
            document.DocumentElement.GetAttribute("lang") ?? throw new ArgumentException("lang attribute should be specified"),
            publicationInfo["publicationcode"].TextContent
        ) { Title = publicationInfo["publicationname"].TextContent.Trim(), };
    }

    private PublicationType GetPublicationType(string type, string subType) =>
        _pubTypeDetector.GetPublicationType(type, subType)
        ?? throw new ArgumentException($"Unknown [{type}:{subType}]");

    private static readonly Dictionary<string, HashSet<string>> RemoveClasses = new()
    {
        ["TABLE"] = new HashSet<string> { "table1", "table" },
        ["H3"] = new HashSet<string> { "chapter" }, // bible
        ["H4"] = new HashSet<string> { "verse" } // bible
    };

    // private readonly IBrowsingContext _context;

    private readonly ServiceProvider _sp;
    private readonly LegacyPublicationTypeDetector _pubTypeDetector;

    private static void RemoveSystemClasses(INode element)
    {
        var emptyHash = new HashSet<string>(0);
        foreach (IElement? descendant in element.DescendentsAndSelf<IElement>())
        {
            HashSet<string> tagRemove = RemoveClasses.GetValueOrDefault(descendant.TagName, defaultValue: emptyHash);
            foreach (string? className in descendant.ClassList.ToList()
                         .Where(className => className.StartsWith("countchapter-", StringComparison.Ordinal)
                                             || className.StartsWith("countpara-", StringComparison.Ordinal)
                                             || className is "infolio" or "notinfolio"
                                             || tagRemove.Contains(className)))
            {
                descendant.ClassList.Remove(className);
            }
        }
    }

    private static void FixNodes(IParentNode root)
    {
        IHtmlCollection<IElement> spans = root.QuerySelectorAll("span.empty");
        foreach (IElement? span in spans)
        {
            span.ReplaceWith(span.ChildNodes.ToArray());
        }

        IHtmlCollection<IElement> divs = root.QuerySelectorAll("divs.empty");
        foreach (IElement? div in divs)
        {
            div.Remove();
        }
    }

    private static IEnumerable<IElement> Combine(IDocument document, IReadOnlyCollection<IElement> group)
    {
        if (group.Count == 1)
        {
            return group;
        }

        var elements = group.ToList();
        var pages = new List<IElement>();
        for (int i = elements.Count - 1; i >= 0; i--)
        {
            IElement currentElement = elements[i];
            if (!currentElement.ClassList.Contains("page"))
            {
                break;
            }

            pages.Add(currentElement);
            elements.RemoveAt(i);
        }

        pages.Reverse();
        List<IElement> regroupedElements = Regroup(document, elements);
        regroupedElements.AddRange(pages);
        return regroupedElements;
    }

    private static List<IElement> Regroup(IDocument document, List<IElement> elements)
    {
        if (!elements.Any())
        {
            return elements;
        }

        var regroupedElements = new List<IElement> { (IElement)elements[0].Clone() };
        IElement lastElement = regroupedElements.Last();
        foreach (IElement? el in elements.Skip(1))
        {
            ITokenList elClassList = el.ClassList;
            if (elClassList.Contains("page")) // append page block to a last one
            {
                IElement pageSpan = document.CreateElement("span");
                pageSpan.ClassName = "page pagenumber";
                pageSpan.InnerHtml = el.TextContent;
                lastElement.AppendChild(document.CreateTextNode(" "));
                lastElement.AppendChild(pageSpan);
            }
            else if (elClassList.Contains("countpara-sameasprevious")) // Flush and start a new paragraph
            {
                lastElement = (IElement)el.Clone();
                regroupedElements.Add(lastElement);
            }
            else // if (elClassList.Contains("countpara-partofprevious")) // Append current to a last one
            {
                lastElement.AppendChild(document.CreateTextNode(" "));
                lastElement.AppendNodes(el.ChildNodes.ToArray());
            }
        }

        return regroupedElements;
    }

    private static IEnumerable<IReadOnlyCollection<IElement>> GroupElements(IEnumerable<IElement> elements)
    {
        var result = new List<IElement>();
        foreach (IElement? element in elements)
        {
            bool startNewGroup;
            ITokenList classList = element.ClassList;
            if (IsPage(classList))
            {
                startNewGroup = false;
            }
            else if (IsPartOfPrevious(classList))
            {
                startNewGroup = PartOfPreviousShouldStartNewGroup(element, result);
            }
            else
            {
                startNewGroup = result.Count > 0;
            }

            if (startNewGroup)
            {
                yield return result;
                result = new List<IElement>();
            }

            result.Add(element);
        }

        if (result.Any())
        {
            yield return result;
        }
    }

    private static bool PartOfPreviousShouldStartNewGroup(IElement element, ICollection<IElement> group)
    {
        IElement? lastNonPageElement = group.LastOrDefault(r => !IsPage(r.ClassList));
        if (lastNonPageElement is null)
        {
            return false;
        }

        return element.TagName != lastNonPageElement.TagName;
    }

    private static bool IsPage(ITokenList classList) => classList.Contains("page");

    private static bool IsPartOfPrevious(ITokenList classList) => classList.Contains("countpara-partofprevious")
                                                                  || classList.Contains("countpara-sameasprevious");


    /// <inheritdoc />
    public void Dispose()
    {
        _sp.Dispose();
        GC.SuppressFinalize(this);
    }
}