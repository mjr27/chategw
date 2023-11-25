using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;

using MediatR;
namespace Egw.PubManagement.Application.Messaging;

/// <inheritdoc />
public abstract class ApplicationCommandHandler<T> : IRequestHandler<T> where T : IApplicationCommand
{
    /// <summary>
    /// Database
    /// </summary>
    protected readonly PublicationDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// Current time
    /// </summary>
    protected DateTimeOffset Now => _clock.Now;

    /// <summary>
    /// Default constructor
    /// </summary>
    protected ApplicationCommandHandler(PublicationDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public abstract Task Handle(T request, CancellationToken cancellationToken);
}