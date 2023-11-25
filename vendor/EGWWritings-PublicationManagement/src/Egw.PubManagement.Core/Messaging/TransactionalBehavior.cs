using System.Diagnostics.CodeAnalysis;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
namespace Egw.PubManagement.Core.Messaging;

/// <summary>
/// Transactional behaviour. Wraps command in transaction
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
[ExcludeFromCodeCoverage]
public class TransactionalBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly DbContext[] _contexts;

    /// <summary>
    /// Create a behavior
    /// </summary>
    public TransactionalBehavior(IServiceProvider provider)
    {
        Type transactionalType = typeof(ITransactionalCommand<>);
        _contexts = typeof(TRequest)
            .GetInterfaces()
            .Where(r => r.IsGenericType)
            .Where(r => r.GetGenericTypeDefinition() == transactionalType)
            .Select(r => r.GenericTypeArguments[0])
            .Select(r => provider.GetRequiredService(r) as DbContext)
            .Where(r => r != null)
            .Select(r => r!)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        IDbContextTransaction[] transactions = await Task
            .WhenAll(_contexts
                .Where(r => r.Database.CurrentTransaction is null)
                .Select(r => r.Database.BeginTransactionAsync(cancellationToken))
            );
        try
        {
            TResponse result = await next();
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.WhenAll(transactions.Select(r => r.RollbackAsync(cancellationToken)));
                return result;
            }

            await Task.WhenAll(transactions.Select(r => r.CommitAsync(cancellationToken)));
            return result;
        }
        catch (Exception)
        {
            await Task.WhenAll(transactions.Select(r => r.RollbackAsync(cancellationToken)));
            throw;
        }
    }
}