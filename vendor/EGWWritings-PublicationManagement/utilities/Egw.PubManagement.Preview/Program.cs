using System.Text.Json;
using System.Text.Json.Serialization;

using CliFx;

using Egw.PubManagement.Core.Messaging;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Core.Validation;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Preview;
using Egw.PubManagement.Preview.Commands;
using Egw.PubManagement.Preview.Services;

using FluentValidation;

using Hellang.Middleware.ProblemDetails;

using Mapster;

using MapsterMapper;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;

builder.Services.AddMvcCore().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    o.JsonSerializerOptions.Converters.Add(new ParaIdConverter());
    o.JsonSerializerOptions.Converters.Add(new ParaIdNullableConverter());
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(namingPolicy: JsonNamingPolicy.CamelCase));
});


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


builder.Services.AddSingleton<IClock, ClockImpl>()
    .AddScoped<IMapper, ServiceMapper>()
    .AddSingleton<TypeAdapterConfig>(_ => new TypeAdapterConfig()
        // .AddPublicationMapping()
    );

builder.Services.AddDbContextFactory<PublicationDbContext>(o => o.UseNpgsql(
        builder.Configuration.GetConnectionString("Publications"),
        p => p.MigrationsAssembly("Egw.PubManagement")
    )
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors());
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PublicationDbContext>();

builder.Services.AddEgwAuthentications(
    builder.Configuration.GetValue<Uri>("Security:Authority")
    ?? new Uri("https://cpanel.egwwritings.org")
);


builder.Services.AddResponseCompression(o =>
{
    o.EnableForHttps = true;
    o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/graphql-response+json" });
});

builder.Services
    .AddTransient<AccessiblePublicationsService>()
    .AddTransient<ParagraphCacheService>()
    .AddTransient<ParagraphConverterService>();
builder.Services.AddMemoryCache();

WebApplication app = builder.Build();
string[] arguments = args;
if (arguments is ["run"])
{
    arguments = Array.Empty<string>();
}

var cliServices = new ServiceCollection();
cliServices.AddSingleton<WebApplication>(_ => app);
cliServices.AddSingleton<IHost>(_ => app);
cliServices.AddTransient<WebServerCommand>();


#pragma warning disable ASP0000
ServiceProvider provider = cliServices.BuildServiceProvider();
#pragma warning restore ASP0000
await new CliApplicationBuilder()
    .UseTypeActivator(provider)
    .AddCommandsFromThisAssembly()
    .Build()
    .RunAsync(arguments);