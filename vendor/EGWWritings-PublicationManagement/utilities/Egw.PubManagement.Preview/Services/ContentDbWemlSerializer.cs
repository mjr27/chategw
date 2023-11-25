using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.BlockChildren;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Links;
using WhiteEstate.DocFormat.Serialization;
namespace Egw.PubManagement.Preview.Services;

internal class ContentDbWemlSerializer : IConverter
{
    private readonly IDocument _document;

    public ContentDbWemlSerializer()
    {
        _document = (IHtmlDocument)BrowsingContext.New().OpenNewAsync().Result;
    }

    public INode Serialize(PublicationType publicationType, IWemlNode node)
    {
        return CreateElement(publicationType, node);
    }

    private INode CreateElement(PublicationType publicationType, IWemlNode node)
    {
        return node switch
        {
            // WemlHeadingContainer weml => CreateHeadingContainer(weml),
            WemlTextNode weml => _document.CreateTextNode(weml.Text),
            WemlAnchorNode weml => _document.CreateElement("a")
                .WithAttribute("name", weml.UniqueId),
            WemlLanguageElement weml => _document.CreateElement("span")
                .WithAttribute("lang", weml.Language)
                .WithEnumAttribute("dir", weml.Direction)
                .WithChildren(MakeChildren(weml.Children, publicationType)),
            WemlEntityElement weml => CreateEntity(publicationType, weml)
                .WithChildren(MakeChildren(weml.Children, publicationType)),
            WemlFormatElement weml => (weml.Formatting switch
            {
                WemlTextFormatting.Bold => _document.CreateElement("strong"),
                WemlTextFormatting.Italic => _document.CreateElement("em"),
                WemlTextFormatting.Subscript => _document.CreateElement("sub"),
                WemlTextFormatting.Superscript => _document.CreateElement("sup"),
                WemlTextFormatting.Underline => _document.CreateElement("u"),
                WemlTextFormatting.AllCaps => _document.CreateElement("span").WithAttribute("class", "caps"),
                WemlTextFormatting.SmallCaps => _document.CreateElement("span").WithAttribute("class", "small-caps"),
                _ => _document.CreateElement("span")
            }).WithChildren(MakeChildren(weml.Children, publicationType)),
            WemlLinkElement weml => CreateLink(weml, publicationType),
            WemlLineBreakNode => _document.CreateElement<IHtmlBreakRowElement>(),
            WemlSentenceElement weml => CreateSentenceElement(weml, publicationType),

            WemlNonEgwElement weml => _document.CreateElement("span")
                .WithAttribute(
                    "class", weml.TextType switch
                    {
                        WemlNonEgwTextType.Appendix => "non-egw-appendix",
                        WemlNonEgwTextType.Comment => "non-egw-comment",
                        WemlNonEgwTextType.Foreword => "non-egw-foreword",
                        WemlNonEgwTextType.Intro => "non-egw-intro",
                        WemlNonEgwTextType.Note => "non-egw-note",
                        WemlNonEgwTextType.Preface => "non-egw-preface",
                        WemlNonEgwTextType.Text => "non-egw-text",
                        _ => null
                    }
                ).WithChildren(MakeChildren(weml.Children, publicationType)),
            WemlNoteNode weml => CreateNote(publicationType, weml),
            WemlPageBreakElement weml => _document.CreateElement("span")
                .WithAttribute("class", "page pagenumber")
                .WithChildren(_document.CreateTextNode(weml.Page)),
            WemlFigureElement weml => _document.CreateElement("p")
                .WithAttribute("class", "picture")
                .WithChildren(_document.CreateElement("a").WithAttribute("href", weml.Source)),
            WemlListElement weml => CreateList(publicationType, weml),
            WemlListItemNode weml => _document.CreateElement<IHtmlListItemElement>()
                .WithChildren(MakeChildren(weml.Children, publicationType)),
            WemlTableElement weml => _document
                .CreateElement("table")
                .WithClass("table1")
                .WithChildren(
                    _document.CreateElement("thead").WithChildren(MakeChildren(weml.Header, publicationType)),
                    _document.CreateElement("tbody").WithChildren(MakeChildren(weml.Body, publicationType))
                ),
            WemlTableRowNode weml => _document.CreateElement("tr")
                .WithChildren(MakeChildren(weml.Cells, publicationType)),
            WemlTableCellNode weml => _document.CreateElement(weml.Header ? "th" : "td")
                .WithEnumAttribute("align", weml.Align)
                .WithEnumAttribute("valign", weml.Valign)
                .WithAttribute("colspan", weml.ColSpan > 1 ? weml.ColSpan.ToString() : null)
                .WithAttribute("rowspan", weml.RowSpan > 1 ? weml.RowSpan.ToString() : null)
                .WithChildren(MakeChildren(weml.Children, publicationType)),
            WemlTextBlockElement weml => CreateTextElement(publicationType, weml),
            WemlThoughtBreakElement => _document.CreateElement("p")
                .WithClass("thoughtbreak"),
            WemlParagraphContainer weml => CreateTextContainer(publicationType, weml),
            WemlParagraphGroupContainer weml => CreateGroupContainer(publicationType, weml),
            WemlHeadingContainer weml => CreateHeadingContainer(publicationType, weml),
            _ => CreateEmptyNode()
        };
    }

    private INode CreateSentenceElement(WemlSentenceElement weml, PublicationType publicationType)
    {
        IDocumentFragment fragment = _document.CreateDocumentFragment();
        fragment.Append(MakeChildren(weml.Children, publicationType));
        return fragment;
    }


    private INode CreateLink(WemlLinkElement weml, PublicationType publicationType)
    {
        IElement link = weml.Link switch
        {
            WemlEmailLink email => _document.CreateElement("a")
                .WithAttribute("href", email.Reference),
            WemlExternalLink wemlHttpLink => _document.CreateElement("a")
                .WithAttribute("href", '#' + wemlHttpLink.Uri.ToString()),
            WemlEgwLink wemlEgwLink => _document.CreateElement("span")
                .WithClass("egwlink")
                .WithClass(wemlEgwLink.IsBible ? "egwlink_bible" : "egwlink_book")
                .WithAttribute("data-link", wemlEgwLink.ParaId.ToString()),
            WemlTemporaryLink => _document.CreateElement("span"),
            _ => CreateEmptyNode()
        };
        return link
            .WithAttribute("title", weml.Title)
            .WithChildren(MakeChildren(weml.Children, publicationType));
    }

    private INode[] MakeChildren(IEnumerable<IWemlNode> nodes, PublicationType publicationType)
    {
        return nodes.Select(r => CreateElement(publicationType, r)).ToArray();
    }

    private IElement CreateEmptyNode()
    {
        return _document.CreateElement("span");
    }

    private IElement CreateTextElement(PublicationType publicationType, WemlTextBlockElement weml)
    {
        return _document.CreateElement(weml.Type == WemlParagraphType.Blockquote ? "blockquote" : "p")
            .WithChildren(MakeChildren(weml.Children, publicationType));
    }

    private INode CreateTextContainer(PublicationType publicationType, WemlParagraphContainer weml)
    {
        if (weml.Child is not WemlTextBlockElement child)
        {
            return CreateElement(publicationType, weml.Child);
        }

        IElement result = CreateTextElement(publicationType, child);
        if (weml.Child is not WemlTextBlockElement paragraph)
        {
            return result;
        }

        result = result
            .WithClass(
                paragraph.Type switch
                {
                    WemlParagraphType.Blockquote => GetBlockquoteClassName(weml.Indent, weml.Align, weml.Role),
                    WemlParagraphType.Paragraph => GetClassName(publicationType, weml.Role, weml.Align, weml.Indent),
                    WemlParagraphType.Poem => GetPoemClassName(weml.Indent),
                    _ => null
                }
            );

        return result;
    }

    private INode CreateGroupContainer(PublicationType publicationType, WemlParagraphGroupContainer weml)
    {
        INode[] children = weml.Children.Select(r => CreateTextContainer(publicationType, r))
            .ToArray();
        INode firstChild = children[0];
        for (int i = 1; i < children.Length; i++)
        {
            firstChild.AppendChild(_document.CreateTextNode(" "));
            firstChild.AppendNodes(children[i].ChildNodes.ToArray());
        }

        return firstChild;
    }

    private INode CreateHeadingContainer(PublicationType publicationType, WemlHeadingContainer weml)
    {
        return _document.CreateElement("h" + Math.Min(weml.Level, 6))
            .WithChildren(MakeChildren(weml.Child.ChildNodes, publicationType));
    }

    private string GetBlockquoteClassName(int level, WemlHorizontalAlign? align, WemlParagraphRole? role)
    {
        if (role is WemlParagraphRole.Introduction)
        {
            return "introquote";
        }

        return align switch
        {
            WemlHorizontalAlign.Center => "blockquote-center",
            WemlHorizontalAlign.Right => "blockquote-right",
            WemlHorizontalAlign.Left => level > 0
                ? "blockquote-indented"
                : "blockquote-noindent",
            _ => level > 0
                ? "blockquote-indented"
                : "blockquote-noindent",
        };
    }

    private static string GetPoemClassName(int indent)
    {
        return indent switch
        {
            0 => "poem-noindent",
            _ => "poem-indented"
        };
    }

    private string? GetClassName(
        PublicationType publicationType,
        WemlParagraphRole? role,
        WemlHorizontalAlign? align,
        int level)
    {
        string Cond(string defaultValue, string ltMsValue) =>
            publicationType == PublicationType.Manuscript
                ? ltMsValue
                : defaultValue;

        return publicationType switch
        {
            PublicationType.Bible => "standard-noindent",
            PublicationType.ScriptureIndex => "standard-noindent",
            PublicationType.TopicalIndex => level switch
            {
                0 => "main",
                1 => "indent1",
                2 => "indent2",
                3 => "indent3",
                4 => "indent4",
                _ => null
            },
            PublicationType.Dictionary => level switch
            {
                1 => "indent1",
                2 => "indent2",
                3 => "indent3",
                4 => "indent4",
                _ => null
            },
            _ => role switch
            {
                WemlParagraphRole.Address => Cond("address", "msaddress"),
                WemlParagraphRole.Addressee => Cond("addressee", "msaddressee"),
                WemlParagraphRole.Author => "periodicalauthor",
                WemlParagraphRole.Date => Cond("date", "msdate"),
                WemlParagraphRole.Place => Cond("place", "msplace"),
                WemlParagraphRole.Introduction => "x-intro",
                WemlParagraphRole.LetterHead => "letterhead",
                WemlParagraphRole.Salutation => Cond("salutation", "mssalutation"),
                WemlParagraphRole.SignatureDate => "signaturedate",
                WemlParagraphRole.SignatureLine => "signatureline",
                WemlParagraphRole.SignatureSource => "signaturesource",
                WemlParagraphRole.BibleText => "bibletext",
                WemlParagraphRole.DevotionalText => "devotionaltext",
                WemlParagraphRole.PoemSource => "poem-source",
                WemlParagraphRole.PublicationInfo => Cond("pubinfo", "mspubinfo"),
                WemlParagraphRole.Title => Cond("title", "mstitle"),
                null => align switch
                {
                    WemlHorizontalAlign.Center => "center",
                    WemlHorizontalAlign.Right => "right",
                    _ => level switch
                    {
                        0 => "standard-noindent",
                        _ => "standard-indented",
                    },
                },
                _ => null
            }
        };
    }

    private INode CreateList(PublicationType publicationType, WemlListElement weml)
    {
        return _document.CreateElement(
            weml.ListType switch
            {
                WemlListType.Ordered => "ol",
                WemlListType.Unordered => "ul",
                _ => "ul"
            }
        ).WithChildren(MakeChildren(weml.Children, publicationType));
    }

    private IElement CreateEntity(PublicationType publicationType, WemlEntityElement weml)
    {
        string? className = weml.EntityType switch
        {
            WemlEntityType.Addressee => "ltaddressee",
            WemlEntityType.Date => "ltdate",
            WemlEntityType.Place => "ltplace",
            WemlEntityType.Topic => publicationType switch
            {
                PublicationType.TopicalIndex => "mindex",
                PublicationType.Dictionary => "h3",
                _ => "topic"
            },
            WemlEntityType.TopicWord => publicationType switch
            {
                PublicationType.TopicalIndex => "lword",
                PublicationType.Dictionary => "h4",
                _ => "word"
            },
            _ => null
        };
        return _document.CreateElement("span")
            .WithClass(className)
            .WithAttribute("start", weml.Value)
            .WithChildren(MakeChildren(weml.Children, publicationType));
    }

    private INode CreateNote(PublicationType publicationType, WemlNoteNode weml)
    {
        string className = weml.Type switch
        {
            WemlNoteType.Footnote => "footnote",
            WemlNoteType.BookEndNote => "bookendnote",
            WemlNoteType.ChapterEndNote => "chapterendnote",
            _ => "footnote"
        };
        IElement span = _document.CreateElement("span")
            .WithClass(className)
            .WithChildren(MakeChildren(weml.Content, publicationType));
        IElement a = _document.CreateElement("a")
            .WithClass(className)
            .WithChildren(MakeChildren(weml.Reference, publicationType).Append(span).ToArray());
        return _document.CreateElement("sup")
            .WithClass(className)
            .WithChildren(a);
    }
}