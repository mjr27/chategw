using Egw.PubManagement.Core.Messaging;
using Egw.PubManagement.Persistence;

using MediatR;
namespace Egw.PubManagement.Application.Messaging;

/// <summary>
/// Application command interface
/// </summary>
public interface IApplicationCommand : IRequest, ITransactionalCommand<PublicationDbContext>
{

}