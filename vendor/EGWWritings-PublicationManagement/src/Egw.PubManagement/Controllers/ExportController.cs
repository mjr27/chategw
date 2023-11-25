using System.Text.Json;
using System.Text.Json.Nodes;

using AngleSharp;

using Egw.PubManagement.Application.Messaging.Export;
using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Json;
using WhiteEstate.DocFormat.Serialization;
namespace Egw.PubManagement.Controllers;

/// <summary>
/// Publication exports
/// </summary>
[ApiController]
[Authorize]
[Route("[controller]/{id:int}")]
public class ExportController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly PublicationDbContext _db;

    /// <inheritdoc />
    public ExportController(IMediator mediator, PublicationDbContext db)
    {
        _mediator = mediator;
        _db = db;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "text/html")]
    public async Task Export([FromRoute] int id, CancellationToken cancellationToken)
    {
        var serializer = new WemlSerializer();
        WemlDocument wemlDocument = await _mediator.Send(new ExportToWemlQuery(id), cancellationToken);
        string html = serializer.Serialize(wemlDocument).ToHtml(new OutputMarkupFormatter());
        Response.ContentType = "text/html";
        await using var writer = new StreamWriter(Response.Body);
        await writer.WriteAsync(html);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "text/html")]
    public async Task ExportJson([FromRoute] int id, CancellationToken cancellationToken)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var serializer = new WemlJsonSerializer();
        WemlDocument wemlDocument = await _mediator.Send(new ExportToWemlQuery(id), cancellationToken);
        Publication publication = await _db.Publications
            .Include(r=>r.Author)
            .SingleAsync(r => r.PublicationId == id, cancellationToken);
        JsonObject json = serializer.Serialize(wemlDocument);
        var meta = (JsonObject)json["meta"]!;
        meta["author"] = publication.Author?.FullName;
        meta["description"] = publication.Description;
        meta["pageCount"] = publication.PageCount;
        meta["isbn"] = publication.Isbn;
        meta["publisher"] = publication.Publisher;
        Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(Response.Body, json, options, cancellationToken);
    }
}