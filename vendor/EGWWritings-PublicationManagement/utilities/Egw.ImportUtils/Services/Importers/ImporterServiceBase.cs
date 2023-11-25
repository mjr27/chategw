using System.Data;

using Egw.PubManagement.Persistence;

namespace Egw.ImportUtils.Services.Importers;

public abstract class ImporterServiceBase : IDisposable
{
    protected readonly PublicationDbContext _db;
    protected readonly IDbConnection _egwDb;

    protected ImporterServiceBase(
        string publicationDbConnectionString,
        string egwConnectionString
    )
    {
        _db = DatabaseUtilities.ConnectToPubDb(publicationDbConnectionString);
        _egwDb = DatabaseUtilities.ConnectToEgwOldDb(egwConnectionString);
    }

    public abstract Task Import(CancellationToken cancellationToken);


    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _db.Dispose();
        _egwDb.Dispose();
    }
}