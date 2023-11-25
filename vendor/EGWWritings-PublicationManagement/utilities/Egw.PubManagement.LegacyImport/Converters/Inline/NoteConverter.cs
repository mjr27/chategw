using AngleSharp.Dom;

using Egw.PubManagement.LegacyImport.Internal;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Inline;

internal class NoteConverter : ILegacyInlineConverter
{
    public bool CanProcess(INode node) => node is IElement
    {
        NodeName: "SUP", ClassName: "chapterendnote" or "bookendnote" or "footnote"
    };


    public IWemlInlineNode Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var element = (IElement)node.Clone();
        IElement? article = element.QuerySelector("span." + element.ClassName);
        if (article is null)
        {
            context.Report(WarningLevel.Error, node, "Note contains no article span");
            return new WemlDummyInlineNode(parser.ParseChildInlines(node, context));
        }

        IElement? link = element.QuerySelector("a." + element.ClassName);
        if (link is null)
        {
            context.Report(WarningLevel.Error, node, "Note contains no link tag");
            return new WemlDummyInlineNode(parser.ParseChildInlines(node, context));
        }

        article.Remove();
        WemlNoteType? noteType = element.ClassName?.ToLower() switch
        {
            "chapterendnote" => WemlNoteType.ChapterEndNote,
            "bookendnote" => WemlNoteType.BookEndNote,
            "footnote" => WemlNoteType.Footnote,
            _ => null
        };

        if (noteType is not null)
        {
            return new WemlNoteNode(
                noteType.Value,
                parser.ParseChildInlines(link, context),
                parser.ParseChildBlocks(article, context)
            );
        }

        context.Report(WarningLevel.Error, node, $"Invalid note type {noteType}");
        return new WemlDummyInlineNode(parser.ParseChildInlines(node, context));

    }
}