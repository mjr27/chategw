using Pgvector;

namespace ChatEgw.UI.Application;

public interface IQueryEmbeddingService
{
    Task<Vector> Embed(string query, CancellationToken cancellationToken);
}