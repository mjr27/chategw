using Microsoft.OpenApi.Models;

using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

using WhiteEstate.DocFormat;
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

internal static class SwaggerExtensions
{

    private static readonly OpenApiSchema ParaIdSchema =
        new() { Type = "string", Pattern = @"^\d{1,8}\.\d{1,8}$", Title = "Paragraph Reference" };

    public static IServiceCollection AddSwaggerFallback(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            foreach (FileInfo xmlFile in new DirectoryInfo(AppContext.BaseDirectory).EnumerateFiles("*.xml"))
            {
                c.IncludeXmlCommentsWithRemarks(xmlFile.FullName);
            }

            c.MapType<ParaId>(() => ParaIdSchema);
            c.MapType<ParaId?>(() => ParaIdSchema);

            c.IncludeXmlCommentsFromInheritDocs();

            c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "CPanel OAuth token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static IApplicationBuilder UseSwaggerInDevelopment(this WebApplication app, string? routePrefix = null)
    {
        // if (!app.Environment.IsDevelopment())
        // {
        //     return app;
        // }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            if (routePrefix != null)
            {
                c.RoutePrefix = routePrefix;
            }

            c.DisplayOperationId();
            c.EnableDeepLinking();
            c.EnableValidator();

            c.OAuthScopes("roles");
            c.EnablePersistAuthorization();

            c.OAuthAppName("EGW Writings Control Panel");

            c.OAuthClientId("x");
            c.OAuthClientSecret("x");
        });
        return app;
    }
}