using System.Diagnostics;

using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Application.Messaging.Import;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Messaging.Commands;

using MediatR;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Serialization;

using Path = System.IO.Path;

namespace Egw.PubManagement.Messaging.CommandHandlers;

/// <inheritdoc />
public class ImportVersionsFromFolderCommandHandler : IRequestHandler<ImportVersionsFromFolderCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ImportVersionsFromFolderCommandHandler> _logger;

    /// <summary>
    /// Default constructor
    /// </summary>
    public ImportVersionsFromFolderCommandHandler(IMediator mediator,
        ILogger<ImportVersionsFromFolderCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Handle(ImportVersionsFromFolderCommand request, CancellationToken cancellationToken)
    {
        var dir = new DirectoryInfo(request.Folder);
        if (!dir.Exists)
        {
            throw new NotFoundProblemDetailsException($"Folder `{dir.FullName}` not found");
        }

        var parser = new HtmlParser();
        var deserializer = new WemlDeserializer();
        FileInfo[] files = dir.EnumerateFiles("*.html")
            .OrderBy(r => int.Parse(Path.GetFileNameWithoutExtension(r.Name)))
            .ToArray();
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];
            _logger.LogInformation("Importing document {Number}/{Total} {Document}",
                i + 1,
                files.Length,
                file.FullName);
            var sw = Stopwatch.StartNew();
            await using FileStream f = file.OpenRead();
            IHtmlDocument htmlDocument = await parser.ParseDocumentAsync(f, cancellationToken);
            htmlDocument.FixImportedHtml();
            WemlDocument wemlDocument = deserializer.Deserialize(htmlDocument);
            await _mediator.Send(new LoadDocumentCommand(wemlDocument, true, request.SkipExisting), cancellationToken);
            _logger.LogInformation("Document {Document} imported in in {Elapsed}", file.FullName, sw.Elapsed);
        }
    }
}