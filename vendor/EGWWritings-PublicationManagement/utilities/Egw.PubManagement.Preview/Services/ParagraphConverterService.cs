using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Services;
using Egw.PubManagement.Preview.Models;
using Egw.PubManagement.Preview.Models.Internal;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
using WhiteEstate.DocFormat.Serialization;
namespace Egw.PubManagement.Preview.Services;

/// <summary>
/// Paragraph converter service
/// </summary>
public class ParagraphConverterService
{
    private readonly ContentDbWemlSerializer _serializer = new();
    private readonly WemlDeserializer _deserializer = new();
    private readonly IHtmlDocument _document = (IHtmlDocument)BrowsingContext.New().OpenNewAsync().Result;

    private IElement ParseWeml(PublicationType type, string content)
    {
        IElement div = _document.CreateElement("div");
        div.InnerHtml = content;
        IWemlNode wemlNode = _deserializer.Deserialize(div.FirstElementChild!);
        INode node = _serializer.Serialize(type, wemlNode);
        return (IElement)node;
    }

    /// <summary>
    /// Project paragraphs
    /// </summary>
    /// <param name="paragraphs"></param>
    /// <returns></returns>
    public IQueryable<ParagraphTemporaryDto> Project(IQueryable<Paragraph> paragraphs) =>
        paragraphs
            .Include(r => r.Publication)
            .Select(r => new ParagraphTemporaryDto
            {
                ParaId = r.ParaId,
                Content = r.Content,
                Order = r.Order,
                PublicationType = r.Publication.Type,
                PublicationCode = r.Publication.Code,
                PublicationTitle = r.Publication.Title,
                Metadata = r.Metadata
            });

    /// <summary>
    /// Converts temporary DTO to output
    /// </summary>
    /// <param name="paragraph"></param>
    /// <returns></returns>
    public LegacyParagraphDto Convert(ParagraphTemporaryDto paragraph)
    {
        IElement htmlElement = ParseWeml(paragraph.PublicationType, paragraph.Content);
        return new LegacyParagraphDto
        {
            ParaId = paragraph.ParaId,
            PubOrder = paragraph.Order,
            RefCode1 = paragraph.PublicationCode,
            RefCode2 = paragraph.Metadata?.Pagination?.Section ?? "",
            RefCode3 =
                paragraph.Metadata is { Pagination.Paragraph: > 0 }
                    ? paragraph.Metadata.Pagination.Paragraph.ToString()
                    : "",
            RefCode4 = "",
            RefCodeLong =
                IRefCodeGenerator.Instance.Long(paragraph.PublicationType, paragraph.PublicationTitle, paragraph.Metadata) ?? "",
            RefCodeShort =
                IRefCodeGenerator.Instance.Short(paragraph.PublicationType, paragraph.PublicationCode, paragraph.Metadata) ?? "",
            ElementType = htmlElement.NodeName.ToLowerInvariant(),
            ElementSubType = htmlElement.ClassName ?? "",
            Content = htmlElement.InnerHtml,
            NextParaId = null!,
            PreviousParaId = null!
        };
    }

}