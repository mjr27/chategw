using Egw.PubManagement.Application.Messaging.Languages;
using Egw.PubManagement.Core.Messaging;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;

using FluentAssertions;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
namespace ApplicationTests.Messaging;

[Collection("Database")]
public abstract class MediatorTestCase : IClassFixture<DatabaseFixture>, IDisposable
{
    private class ClockImpl : IClock
    {

        public DateTimeOffset Now { get; set; }
    }

    private readonly ClockImpl _clock = new();
    private readonly IDbContextTransaction _t;
    private readonly ServiceProvider _provider;
    protected PublicationDbContext Db { get; }
    protected DateTimeOffset Now { get => _clock.Now; set => _clock.Now = value; }


    protected MediatorTestCase(DatabaseFixture fixture)
    {
        Db = fixture.Db;
        _t = Db.Database.BeginTransaction();
        var collection = new ServiceCollection();
        collection.AddSingleton<IClock>(_clock);
        collection.AddSingleton(Db);
        collection.AddValidatorsFromAssemblyContaining<CreateLanguageHandler>();
        collection.AddMediatR(o => o.RegisterServicesFromAssemblyContaining<CreateLanguageHandler>()
            .AddOpenBehavior(typeof(FluentValidationBehavior<,>)));
        _provider = collection.BuildServiceProvider();
    }

    protected async Task ShouldThrow<T>(IRequest request) where T : Exception =>
        await this.Awaiting(_ => this.Execute(request)).Should().ThrowAsync<T>();

    protected async Task ShouldThrow(IRequest request, Type? exception, Func<Task> handleSuccess)
    {
        if (exception is null)
        {
            await Execute(request);
            await handleSuccess();
        }
        else
        {
            await ShouldThrow(request, exception);
        }
    }

    protected async Task ShouldThrow(IRequest request, Type type)
    {
        try
        {
            await Execute(request);
        }
        catch (Exception e)
        {
            e.Should().BeOfType(type);
        }
    }

    protected Task Execute<T>(T request) where T : IRequest => _provider.GetRequiredService<IMediator>().Send(request);

    protected Task<TOutput> Execute<TInput, TOutput>(TInput request) where TInput : IRequest<TOutput> =>
        _provider.GetRequiredService<IMediator>().Send(request);


    public virtual void Dispose()
    {
        _provider.Dispose();
        Db.ChangeTracker.Clear();
        _t.Rollback();
        _t.Dispose();
        GC.SuppressFinalize(this);
    }
}