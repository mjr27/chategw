using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace Egw.PubManagement.Persistence;

/// <summary>
/// Database extensions
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Inserts chunked data into the database
    /// </summary>
    /// <param name="db">Database</param>
    /// <param name="items">Enumerable of items</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="chunkCount">Chunk count</param>
    /// <typeparam name="T"></typeparam>
    public static async Task ChunkedInsertAsync<T>(this DbContext db,
        IEnumerable<T> items,
        CancellationToken cancellationToken = new(),
        int chunkCount = 5000) where T : class
    {
        Task? currentTask = null;
        DbSet<T> context = db.Set<T>();
        foreach (T[] rows in items.Chunk(chunkCount))
        {
            if (currentTask is not null)
            {
                await currentTask;
            }

            currentTask = SaveRows(db, context, rows, false, cancellationToken);
        }

        if (currentTask is not null)
        {
            await currentTask;
        }
    }

    private static async Task SaveRows<T>(DbContext db, DbSet<T> dbSet, T[] rows,
        bool startTransaction,
        CancellationToken cancellationToken
    )
        where T : class
    {
        IDbContextTransaction? transaction = null;
        if (startTransaction)
        {
            transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        }

        dbSet.AddRange(rows);
        await db.SaveChangesAsync(cancellationToken);
        db.ChangeTracker.Clear();
        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
            await transaction.DisposeAsync();
        }
    }
}
