using Egw.ImportUtils.Messaging.Commands;
using Egw.ImportUtils.Services;
using Egw.PubManagement.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MySqlConnector;

namespace Egw.ImportUtils.Messaging.CommandHandlers;

/// <inheritdoc />
public class ExportMultiplePublicationsFromEgwOldCommandHandler
    : IRequestHandler<ExportMultiplePublicationsFromEgwOldCommand>
{
    private readonly PublicationDbContext _db;
    private readonly ILogger<ExportMultiplePublicationsFromEgwOldCommandHandler> _logger;
    private readonly BookExporter _exporter;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="db">Database connection</param>
    /// <param name="logger">Logger</param>
    public ExportMultiplePublicationsFromEgwOldCommandHandler(PublicationDbContext db,
        ILogger<ExportMultiplePublicationsFromEgwOldCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
        _exporter = new BookExporter();
    }

    /// <inheritdoc />
    public async Task Handle(ExportMultiplePublicationsFromEgwOldCommand request,
        CancellationToken cancellationToken)
    {
        List<int> availableBooks = await _db.PublicationPlacement
            .Select(r => r.PublicationId)
            .Where(r => r >= request.Min && r <= request.Max)
            .ToListAsync(cancellationToken);
        await using var egwOldConnection = new MySqlConnection(request.ConnectionString);

        var dir = new DirectoryInfo(request.OutputFolder);
        if (!dir.Exists)
        {
            dir.Create();
        }

        foreach (int bookId in availableBooks.OrderBy(r => r))
        {
            string outputFilename = Path.Combine(request.OutputFolder, $"{bookId}.html");
            if (File.Exists(outputFilename))
            {
                if (request.SkipExisting)
                {
                    continue;
                }

                File.Delete(outputFilename);
            }

            _logger.LogInformation("Exporting {BookId} to {OutputFilename}", bookId, outputFilename);
            await using var writer = new StreamWriter(outputFilename);
            await _exporter.ExportPublication(egwOldConnection, bookId, writer);
        }
    }
}