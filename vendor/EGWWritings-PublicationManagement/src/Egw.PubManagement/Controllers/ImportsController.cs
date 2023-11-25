using System.IO.Compression;

using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Application.Messaging.Import;
using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core.Problems;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Serialization;

using Path = System.IO.Path;

namespace Egw.PubManagement.Controllers;

/// <summary>
/// Publication imports
/// </summary>
[ApiController]
[Route("[controller]")]
public class ImportController : ControllerBase
{
    private readonly IBackgroundTaskQueue _importService;

    /// <inheritdoc />
    public ImportController(IBackgroundTaskQueue importService)
    {
        _importService = importService;
    }

    /// <summary>
    /// Uploads new documents into storage
    /// </summary>
    /// <param name="files">List of html documents to upload</param>
    /// <param name="cancellationToken"></param>
    [Authorize]
    [HttpPost]
    [RequestSizeLimit(200_000_000)]
    public async Task Upload([FromForm(Name = "file")] IFormFile[] files, CancellationToken cancellationToken)
    {
        string tempFileName = Path.GetTempFileName();
        foreach (IFormFile? file in files)
        {
            await using (FileStream f = System.IO.File.Create(tempFileName))
            {
                await file.CopyToAsync(f, cancellationToken);
            }


            _importService.AddTask(Guid.NewGuid(), "Importing publication", MakeTask(new FileInfo(tempFileName)));
        }
    }

    /// <summary>
    /// Validates WEML document
    /// </summary>
    /// <param name="file">File to validate</param>
    /// <param name="cancellationToken"></param>
    [Authorize]
    [HttpPost("validate")]
    [RequestSizeLimit(200_000_000)]
    public async Task<IActionResult> Validate(IFormFile file, CancellationToken cancellationToken)
    {
        var parser = new HtmlParser();
        var deserializer = new WemlDeserializer();
        await using Stream f = file.OpenReadStream();
        IHtmlDocument htmlDocument = await parser.ParseDocumentAsync(f, cancellationToken);
        htmlDocument.FixImportedHtml();
        try
        {
            deserializer.Deserialize(htmlDocument);
        }
        catch (DeserializationException e)
        {
            throw new ValidationProblemDetailsException($"{e.Message}:\n{e.Node?.ToHtml()}");
        }

        return NoContent();
    }

    /// <summary>
    /// Import folder
    /// </summary>
    /// <param name="file">Zip file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [Authorize]
    [HttpPost("zip")]
    [RequestSizeLimit(200_000_000)]
    public async Task ImportZip(IFormFile file,
        CancellationToken cancellationToken = new())
    {
        string tempFileName = Path.GetTempFileName();
        try
        {
            await using (FileStream f = System.IO.File.Create(tempFileName))
            {
                await file.CopyToAsync(f, cancellationToken);
            }

            using ZipArchive zip = ZipFile.Open(tempFileName, ZipArchiveMode.Read);
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                string tempFile = Path.GetTempFileName();
                entry.ExtractToFile(tempFile, true);
                _importService.AddTask(Guid.NewGuid(), "Importing publication from zip file",
                    MakeTask(new FileInfo(tempFile)));
            }
        }
        finally
        {
            if (System.IO.File.Exists(tempFileName))
            {
                System.IO.File.Delete(tempFileName);
            }
        }

        // await _mediator.Send(new ImportZipCommand(tempFileName, skipExisting), cancellationToken);
    }

    private Func<IServiceProvider, CancellationToken, Task> MakeTask(FileInfo file)
    {
        return async (provider, ct) =>
        {
            var parser = new HtmlParser();
            var deserializer = new WemlDeserializer();
            using IServiceScope scope = provider.CreateScope();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await using Stream f = file.OpenRead();
            IHtmlDocument htmlDocument = await parser.ParseDocumentAsync(f, ct);
            htmlDocument.FixImportedHtml();
            WemlDocument wemlDocument = deserializer.Deserialize(htmlDocument);
            await mediator.Send(new LoadDocumentCommand(wemlDocument, true, false), ct);
        };
    }

   
}