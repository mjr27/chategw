using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;

using MediatR;

using Microsoft.Extensions.Logging;
namespace Egw.PubManagement.Application.Messaging.Import;

/// <inheritdoc />
public class RecalculatePublicationMetadataHandler : IRequestHandler<RecalculatePublicationMetadataCommand>
{
    private readonly MetadataGenerator _generator;
    private readonly PublicationDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// Command handler
    /// </summary>
    public RecalculatePublicationMetadataHandler(
        PublicationDbContext db,
        ILoggerFactory loggerFactory,
        IClock clock)
    {
        _generator = new MetadataGenerator(db, loggerFactory.CreateLogger<MetadataGenerator>());
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task Handle(RecalculatePublicationMetadataCommand request, CancellationToken cancellationToken)
    {
        await _generator.FillPublicationMetadata(request.PublicationId, _clock.Now, cancellationToken);
        _db.ChangeTracker.Clear();
    }
}