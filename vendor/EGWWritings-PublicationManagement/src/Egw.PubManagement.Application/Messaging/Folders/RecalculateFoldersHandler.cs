using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
namespace Egw.PubManagement.Application.Messaging.Folders;

/// <inheritdoc />
public class RecalculateFoldersHandler : ApplicationCommandHandler<RecalculateFoldersInput>
{
    private readonly PublicationDbContext _dbContext;
    private readonly IClock _clock;

    /// <summary>
    /// Recalculate folders command handler
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="clock"></param>
    public RecalculateFoldersHandler(PublicationDbContext dbContext, IClock clock) : base(dbContext, clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    /// <inheritdoc />
    public async override Task Handle(RecalculateFoldersInput request, CancellationToken cancellationToken)
    {
        await _dbContext.RecalculateFolders(_clock.Now, cancellationToken);
    }
}