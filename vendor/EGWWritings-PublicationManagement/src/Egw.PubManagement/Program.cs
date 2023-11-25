using CliFx;

using Egw.PubManagement.Application;
using Egw.PubManagement.Application.Services;
using Egw.PubManagement.Core;
using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Core.Messaging;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Core.Validation;
using Egw.PubManagement.EpubExport.Services;
using Egw.PubManagement.GraphQl;
using Egw.PubManagement.LatexExport;
using Egw.PubManagement.LegacyImport.LinkRepository;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Services;
using Egw.PubManagement.Services.Latex;
using Egw.PubManagement.Storage;

using FluentValidation;

using Hellang.Middleware.ProblemDetails;

using Mapster;

using MapsterMapper;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;

builder.Services.AddControllers();

builder.Services.AddRouting(o =>
{
    o.LowercaseUrls = true;
    o.LowercaseQueryStrings = true;
    o.AppendTrailingSlash = false;
}).Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 200_000_000);

builder.Services.AddSwaggerFallback();

ProblemDetailsExtensions.AddProblemDetails(builder.Services);
builder.Services.AddMediatR(o => o.RegisterServicesFromAssemblyContaining<Program>()
    .AddOpenBehavior(typeof(TransactionalBehavior<,>))
    .AddOpenBehavior(typeof(FluentValidationBehavior<,>)));

builder.Services.AddTransient<Mp3ManifestService>();

builder.Services
    .AddGraphQLServer()
    .ModifyOptions(o =>
    {
        o.RemoveUnreachableTypes = true;
        o.UseXmlDocumentation = true;
    })
    .InitializeOnStartup()
    .AddProjections()
    .AddFiltering<CustomFilteringConvention>()
    .AddSorting()
    .AddAuthorization()
    .AddErrorFilter<ProblemErrorFilter>()
    .AddMutationConventions()
    .ModifyRequestOptions(r =>
    {
        r.IncludeExceptionDetails = true;
        r.ExecutionTimeout = TimeSpan.FromSeconds(60);
    })
    .AddQueryType<GraphQlQueries>()
    .AddMutationType<GraphQlMutations>()
    .AddTypeExtension<QueueExtensions>()
    .AddJsonArrayConventions()
    .AddDiagnosticEventListener<ErrorLoggingDiagnosticsEventListener>()
    .AddPublicationsPart()
    .AddType<UploadType>();

builder.Services
    .AddHttpClient()
    .AddSingleton<IClock, ClockImpl>()
    .AddScoped<IMapper, ServiceMapper>()
    .AddSingleton<ICoverFetcher, CoverFetcher>()
    .AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueueImpl>()
    .AddHostedService(p => p.GetRequiredService<IBackgroundTaskQueue>())
    .AddSingleton<TypeAdapterConfig>(_ => new TypeAdapterConfig()
        .AddPublicationMapping(builder.Configuration)
    );

builder.Services.AddHealthChecks()
    .AddDbContextCheck<PublicationDbContext>();

builder.Services.AddSingleton<IStorageWrapper>(_ =>
        new StorageWrapper(
            new BlobFileStorage(builder.Configuration.GetConnectionString("Covers")!),
            new BlobFileStorage(builder.Configuration.GetConnectionString("Exports")!),
            new BlobFileStorage(builder.Configuration.GetConnectionString("Mp3")!)
        ))
    .AddSingleton<ILatexCoverRepository, DatabaseLatexCoverRepository>()
    .AddSingleton<ILatexPublicationRepository, DatabaseLatexPublicationRepository>();
builder.Services.AddSingleton<IImageProxyWrapper>(_ =>
    new ImageProxyWrapperImpl(builder.Configuration.GetConnectionString("ImgProxy")!));

builder.Services.AddEgwAuthentications(
    builder.Configuration.GetValue<Uri>("Security:Authority")
    ?? new Uri("https://cpanel.egwwritings.org")
);


builder.Services.AddPublicationsPart(o =>
{
    o.UseNpgsql(
            builder.Configuration.GetConnectionString("Publications"),
            p => p.MigrationsAssembly("Egw.PubManagement")
        )
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();
});

builder.Services.AddResponseCompression(o =>
{
    o.EnableForHttps = true;
    o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/graphql-response+json" });
});

builder.Services.AddTransient<ILinkRepository, DatabaseJsonLinkRepository>();

WebApplication app = builder.Build();
string[] arguments = args;
if (arguments is ["run"])
{
    arguments = Array.Empty<string>();
}

var cliServices = new ServiceCollection();
cliServices.AddSingleton<WebApplication>(_ => app);
cliServices.AddSingleton<IHost>(_ => app);
cliServices.AddCliFxFromAssemblies(typeof(Program).Assembly);

#pragma warning disable ASP0000
ServiceProvider provider = cliServices.BuildServiceProvider();
#pragma warning restore ASP0000
await new CliApplicationBuilder()
    .UseTypeActivator(provider)
    .AddCommandsFromThisAssembly()
    .Build()
    .RunAsync(arguments);