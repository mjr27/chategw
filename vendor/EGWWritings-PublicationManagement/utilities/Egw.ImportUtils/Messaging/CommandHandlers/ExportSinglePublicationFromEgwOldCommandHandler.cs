using Egw.ImportUtils.Messaging.Commands;
using Egw.ImportUtils.Services;

using MediatR;

using MySqlConnector;

namespace Egw.ImportUtils.Messaging.CommandHandlers;

/// <inheritdoc />
public class ExportSinglePublicationFromEgwOldCommandHandler : IRequestHandler<ExportSinglePublicationFromEgwOldCommand>
{
    private readonly BookExporter _exporter;

    /// <summary>
    /// Default constructor
    /// </summary>
    public ExportSinglePublicationFromEgwOldCommandHandler()
    {
        _exporter = new BookExporter();
    }

    /// <inheritdoc />
    public async Task Handle(ExportSinglePublicationFromEgwOldCommand request,
        CancellationToken cancellationToken)
    {
        await using var egwOldConnection = new MySqlConnection(request.ConnectionString);
        await using var writer = new StreamWriter(request.OutputStream);
        await _exporter.ExportPublication(egwOldConnection, request.PublicationId, writer);
    }
}