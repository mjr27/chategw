using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;

namespace Egw.PubManagement.Application.Messaging.Mp3;

/// <inheritdoc />
public class UpdateMp3ManifestHandler : ApplicationCommandHandler<UpdateMp3ManifestInput>
{
    private readonly Mp3ManifestService _manifestService;

    /// <inheritdoc />
    public UpdateMp3ManifestHandler(Mp3ManifestService manifestService, PublicationDbContext db,
        IClock clock) : base(db, clock)
    {
        _manifestService = manifestService;
    }

    /// <inheritdoc />
    public async override Task Handle(UpdateMp3ManifestInput request, CancellationToken cancellationToken)
    {
        await _manifestService.UpdateManifest(request, cancellationToken);
    }
}