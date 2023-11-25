using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Core.Messaging;

/// <summary>
/// Base transactional command
/// </summary>
// ReSharper disable once UnusedTypeParameter
public interface ITransactionalCommand<TContext>
    where TContext : DbContext
{
}