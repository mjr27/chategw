using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using Egw.PubManagement.LatexExport.Internal;
using Egw.PubManagement.LatexExport.Models;
using Egw.PubManagement.LatexExport.Nodes;

using WhiteEstate.DocFormat.Enums;

namespace Egw.PubManagement.LatexExport.Converter;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
internal class LatexBuilder
{
    private readonly LatexPublicationDto _publication;
    private readonly FileInfo? _cover;
    private readonly IEnumerable<ILatexHeadingTransformer> _transformers;
    private readonly LatexPublicationOptions _options;
    private readonly HtmlParser _parser;
    private readonly IHtmlElement _root;
    private ILatexNode? _lastPageNode;
    private bool _headingFound;
    private bool _hasEndNotes;

    public LatexBuilder(LatexPublicationDto publication,
        FileInfo? cover,
        LatexPublicationOptions? options,
        ILatexHeadingTransformer[] transformers) : this(publication, cover, options, transformers.AsEnumerable())
    {
    }

    public LatexBuilder(LatexPublicationDto publication,
        FileInfo? cover,
        LatexPublicationOptions? options,
        IEnumerable<ILatexHeadingTransformer> transformers)
    {
        _options = options ?? new();
        _publication = publication;
        _cover = cover;
        _transformers = transformers.ToArray();
        _parser = new HtmlParser();
        using IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        using IDocument document = context.OpenNewAsync().Result;
        _root = document.Body!;
        _lastPageNode = null;
        _headingFound = false;
    }

    public async IAsyncEnumerable<ILatexNode> ConvertDocument(
        IAsyncEnumerable<LatexParagraphDto> paragraphs,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string languageCode = _publication.LanguageCode;
        yield return new LatexMacroNode("documentclass", GetDocumentClass(_publication.Type))
        {
            Options = { ["lang=" + languageCode] = null }
        };
        yield return new LatexMacroNode("usepackage", "geometry")
        {
            Options =
            {
                { "a5paper", null }, { "marginparwidth", "5cm" }, { "marginparsep", "3mm" }
            }, // TODO pass in options
        };
        yield return new LatexMacroNode("title", _publication.Title);
        if (_publication.Author is not null)
        {
            yield return new LatexMacroNode("author", _publication.Author);
        }

        yield return new LatexMacroNode("date", _publication.PublicationYear?.ToString() ?? "");

        if (_cover is { Exists: true })
        {
            yield return new LatexMacroNode("cover", _cover.Name.EscapeLatex());
        }
        else
        {
            Console.WriteLine(_cover?.FullName ?? null);
            Console.WriteLine("Does not exist");
        }

        yield return new LatexMacroNode("begin", "document");
        yield return new LatexMacroNode("begin", "egwbody");


        if (_options.InsertTocBefore is null)
        {
            yield return new LatexMacroNode("wToc");
        }

        await foreach (LatexParagraphDto paragraph in paragraphs.WithCancellation(cancellationToken))
        {
            foreach (ILatexNode node in ProcessParagraph(paragraph))
            {
                yield return node;
            }
        }

        yield return MakeEndNoteList();
        yield return new LatexMacroNode("end", "egwbody");
        yield return new LatexMacroNode("end", "document");
    }

    private IEnumerable<ILatexNode> ProcessParagraph(LatexParagraphDto paragraph)
    {
        if (paragraph.HeadingLevel == 1)
        {
            yield break;
        }

        if (paragraph.ParagraphId == _options.InsertTocBefore)
        {
            yield return new LatexMacroNode("wToc");
        }

        IElement root = _parser.ParseFragment(paragraph.WemlContent, _root)
            .OfType<IElement>()
            .Single();
        ILatexNode node = ParseWeml(root);
        if (node is LatexMacroNode { Name: "wHeading" } headingNode)
        {
            headingNode.Options["id"] = paragraph.ParagraphId.ToString();
        }

        if (_headingFound || node is not LatexMacroNode macroNode || ProcessMacroNode(macroNode))
        {
            yield return node;
        }
    }

    private bool ProcessMacroNode(LatexMacroNode macroNode)
    {
        switch (macroNode)
        {
            case { Name: "wPage" }:
                _lastPageNode = macroNode;
                return false;
            case { Name: "wHeading" }
                when macroNode.Options.TryGetValue("level", out string? headingLevelStr)
                     && int.TryParse(headingLevelStr, out int headingLevel)
                     && headingLevel > 1:
                {
                    _headingFound = true;
                    if (_lastPageNode is not null)
                    {
                        macroNode.Children.Insert(0, _lastPageNode);
                        _lastPageNode = null;
                    }

                    break;
                }
        }

        return true;
    }

    private static string GetDocumentClass(PublicationType publicationType)
    {
        return publicationType switch
        {
            PublicationType.Book => "egw_book",
            _ => throw new ArgumentOutOfRangeException(nameof(publicationType), publicationType, null)
        };
    }

    private ILatexNode ParseWeml(INode root)
    {
        switch (root)
        {
            case IElement element:
                return ParseWemlElement(element);
            case { NodeType: NodeType.Text }:
                return new LatexTextNode(root.TextContent.EscapeLatex());
            default:
                throw new ArgumentOutOfRangeException("Unknown node type: " + root.NodeType);
        }
    }

    private ILatexNode ParseWemlElement(IElement root)
    {
        switch (root)
        {
            case { NodeName: "W-PAGE" }:
                return new LatexMacroNode("wPage",
                    root.GetAttribute("number") ?? throw new InvalidOperationException("Missing page number"));
            case { NodeName: "W-HEADING" }:
                return MakeHeading(root);
            case { NodeName: "W-TEXT-BLOCK" }:
                return MakeNode("wTextBlock", root);
            case { NodeName: "W-PARA" }:
                return MakeNode("wPara", root);
            case { NodeName: "W-FORMAT" }:
                return MakeNode("wFormat", root);
            case { NodeName: "W-NON-EGW" }:
                return MakeNode("wNonEgw", root);
            case { NodeName: "HR" }:
                return new LatexMacroNode("wHr");
            case { NodeName: "BR" }:
                return new LatexMacroNode("wBr");
            case { NodeName: "W-NOTE" }:
                IElement child = root.QuerySelector("w-note-body") ??
                                 throw new InvalidOperationException("Missing w-note-body");
                _hasEndNotes = true;
                return new LatexMacroNode("wNote")
                {
                    Options = { ["type"] = root.GetAttribute("type") }, Children = ParseWemlChildren(child)
                };
            case { NodeName: "A" } when !string.IsNullOrWhiteSpace(root.GetAttribute("href")):
                return MakeLink(root.GetAttribute("href")!, root);

            case { NodeName: "W-SENT" }:
                return new LatexMacroNode("wSent") { Children = ParseWemlChildren(root) };
            case { NodeName: "W-PARA-GROUP" }:
                return new LatexMacroNode("wParaGroup") { Children = ParseWemlChildren(root) };
            // TABLE
            case { NodeName: "TABLE" }:
                return MakeNode("wTable", root);
            case { NodeName: "THEAD" }:
                return MakeNode("wTableHeader", root);
            case { NodeName: "TBODY" }:
                return MakeNode("wTableBody", root);
            case { NodeName: "TR" }:
                return MakeNode("wTableRow", root);
            case { NodeName: "TD" }:
                return MakeNode("wTableCell", root);
            default:
                Console.WriteLine(root.ToHtml());
                throw new ArgumentOutOfRangeException("Unknown element: " + root.NodeName);
        }
    }

    private ILatexNode MakeEndNoteList()
    {
        if (!_hasEndNotes)
        {
            return new LatexTextNode("");
        }

        _hasEndNotes = false;
        return new LatexMacroNode("wEndNotes");
    }

    private ILatexNode MakeHeading(IElement root)
    {
        int level = int.Parse(root.GetAttribute("level")!) - 1;
        ILatexNode node = new LatexMacroNode("wHeading")
        {
            Children = ParseWemlChildren(root),
            Options =
            {
                ["level"] = level.ToString(),
                ["label"] = root.TextContent.Trim(),
                ["mark"] = ExtractLabel(root.TextContent)
            }
        };
        if (level <= _options.EndNoteLevel)
        {
            node = new LatexGroupNode(MakeEndNoteList(), node);
        }

        return node;
    }

    private ILatexNode MakeLink(string href, IElement root)
    {
        if (!Uri.TryCreate(href, UriKind.Absolute, out Uri? uri) || uri is { Scheme: "http" or "https" })
        {
            return MakeAbsoluteLink(href, root);
        }

        if (uri is not { Scheme: "egw" })
        {
            throw new ArgumentOutOfRangeException("Invalid link: " + href);
        }

        string endPath = uri.Segments.Last();
        return MakeAbsoluteLink("https://egw.bz/p/" + endPath, root);
    }

    private ILatexNode MakeAbsoluteLink(string href, INode root)
    {
        return new LatexMacroNode("wLink")
        {
            Options = { ["href"] = href.EscapeLatex() }, Children = ParseWemlChildren(root)
        };
    }

    private LatexMacroNode MakeNode(string name, IElement element)
    {
        return new LatexMacroNode(name) { Options = ParseAttributes(element), Children = ParseWemlChildren(element), };
    }

    private List<ILatexNode> ParseWemlChildren(INode root)
    {
        return root.ChildNodes.Select(ParseWeml).ToList();
    }

    private static Dictionary<string, string?> ParseAttributes(IElement root)
    {
        return root.Attributes.ToDictionary(attribute => attribute.Name, attribute => (string?)attribute.Value);
    }

    private string ExtractLabel(string text)
    {
        text = text.Trim();
        foreach (ILatexHeadingTransformer transformer in _transformers)
        {
            text = transformer.TrimHeading(_publication.LanguageCode, text);
        }

        return text;
    }
}