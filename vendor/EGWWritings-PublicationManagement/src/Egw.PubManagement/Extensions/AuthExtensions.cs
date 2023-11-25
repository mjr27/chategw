using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

internal static class AuthExtensions
{
    internal static IServiceCollection AddEgwAuthentications(this IServiceCollection services, Uri authority)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.Authority = authority.AbsoluteUri.TrimEnd('/');
                o.TokenValidationParameters.ValidateAudience = false;
                o.TokenValidationParameters.ValidateIssuer = true;
                o.IncludeErrorDetails = true;
                o.SaveToken = true;
                o.Validate();
            });
        services.AddAuthorization(o =>
        {
            o.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(ClaimTypes.NameIdentifier)
                .RequireRole("Admin")
                .Build();
        });

        services.AddCors(
            cors => cors.AddPolicy("API",
                b => b.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
            )
        );
        return services;
    }
}