using AngleSharp.Dom;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Containers;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport.Converters.Container;

internal class ParagraphContainerConverter : ILegacyContainerConverter
{
    private record ParagraphAttributes(
        WemlHorizontalAlign? Alignment,
        WemlParagraphRole? Role,
        int Level
    );

    public bool CanProcess(INode node) => node is IElement { NodeName: "P" or "DIV" } el
                                          && !el.ClassList.Contains("page");

    public IWemlContainerElement Convert(ILegacyElementParser parser, INode node, ILegacyParserContext context)
    {
        var el = (IElement)node;
        ParagraphAttributes paragraphDetails = GetParagraphAttributes(el, context);

        IWemlBlockNode? element = parser.ParseBlock(node, context);

        if (element is not null)
        {
            return new WemlParagraphContainer(element)
            {
                Role = paragraphDetails.Role, Align = paragraphDetails.Alignment, Indent = paragraphDetails.Level
            };
        }

        context.Report(WarningLevel.Error, node, "Cannot parse paragraph inline");
        element = new WemlTextBlockElement(WemlParagraphType.Paragraph, ArraySegment<IWemlInlineNode>.Empty);

        return new WemlParagraphContainer(element)
        {
            Role = paragraphDetails.Role, Align = paragraphDetails.Alignment, Indent = paragraphDetails.Level
        };
    }


    // ReSharper disable once CognitiveComplexity
    private ParagraphAttributes GetParagraphAttributes(IElement element, ILegacyParserContext context)
    {
        WemlHorizontalAlign? align = null;
        WemlParagraphRole? role = null;
        int level = 1;

        foreach (string? className in element.ClassList)
        {
            switch (className)
            {
                case "left":
                    align = WemlHorizontalAlign.Left;
                    break;
                case "center":
                    align = WemlHorizontalAlign.Center;
                    break;
                case "right":
                    align = WemlHorizontalAlign.Right;
                    break;
                case "ltaddress":
                case "msaddress":
                    role = WemlParagraphRole.Address;
                    break;
                case "addressee":
                case "ltaddressee":
                case "msaddressee":
                    role = WemlParagraphRole.Addressee;
                    break;
                case "bibletext":
                    role = WemlParagraphRole.BibleText;
                    break;
                case "date":
                case "ltdate":
                case "msdate":
                    role = WemlParagraphRole.Date;
                    break;
                case "devotionaltext":
                    role = WemlParagraphRole.DevotionalText;
                    break;
                case "letterhead":
                    role = WemlParagraphRole.LetterHead;
                    break;
                case "periodicalauthor":
                    role = WemlParagraphRole.Author;
                    break;
                case "place":
                case "ltplace":
                case "msplace":
                case "signatureplace":
                    role = WemlParagraphRole.Place;
                    break;
                case "poem-source":
                    role = WemlParagraphRole.PoemSource;
                    break;
                case "salutation":
                case "ltsalutation":
                case "mssalutation":
                    role = WemlParagraphRole.Salutation;
                    break;
                case "signaturedate":
                    role = WemlParagraphRole.SignatureDate;
                    break;
                case "signatureline":
                case "egwsignatureline":
                    role = WemlParagraphRole.SignatureLine;
                    break;
                case "signaturesource":
                    role = WemlParagraphRole.SignatureSource;
                    break;
                case "mstitle":
                    role = WemlParagraphRole.Title;
                    break;
                case "ltpubinfo":
                case "mspubinfo":
                    role = WemlParagraphRole.PublicationInfo;
                    break;

                case "standard-indented":
                    level = 1;
                    break;
                case "already-indented":
                case "standard-noindent":
                    level = 0;
                    break;

                case "reverse-indented":
                case "reverse-indent1":
                case "reverse-indent2":
                case "reverse-indent3":
                case "reverse-2ndLine":
                case "reverse-2ndLine1":
                    // indent = IndentType.ReverseIndented;
                    break;

                case "blockquote-noindent":
                    level = 0;
                    break;
                case "blockquote-insideaddr":
                case "blockquote-indented":
                    level = 1;
                    break;

                case "blockquote-reverse":
                    // indent = IndentType.ReverseIndented;
                    break;

                case "blockquote-center":
                    align = WemlHorizontalAlign.Center;
                    break;

                case "poem-indented":
                    level = 1;
                    break;

                case "poem-noindent":
                    level = 0;
                    break;
                case "thoughtbreak":
                    break;
                case "picture":
                case "wrap" or "nowrap" or "wrap-start" or "wrap-end": // EGWEnc
                    break;
                case "introquote":
                    role = WemlParagraphRole.Introduction;
                    break;

                // Topical index and dictionaries
                case "main":
                    level = 0;
                    break;
                case "indent0":
                    level = 0;
                    break;
                case "indent1":
                case "indend1":
                    level = 1;
                    break;
                case "indent2":
                case "indend2":
                    level = 2;
                    break;
                case "indent3":
                case "indend3":
                    level = 3;
                    break;
                case "indent4":
                case "indend4":
                    level = 4;
                    break;
                case "list":
                    level = 0;
                    break;
                case "list0":
                    level = 0;
                    break;
                case "list1":
                    level = 1;
                    break;
                default:
                    context.Report(WarningLevel.Error, element, "Invalid class name");
                    break;
            }
        }

        return new ParagraphAttributes(align, role, level);
    }
}