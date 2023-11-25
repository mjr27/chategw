using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Hellang.Middleware.ProblemDetails;

namespace Egw.PubManagement.Commands;

/// <summary>
/// Web server command
/// </summary>
[Command(Description = "Runs web server")]
public class WebServerCommand : ICommand
{
    private readonly WebApplication _application;

    /// <summary>
    /// Default constructor
    /// </summary>
    public WebServerCommand(WebApplication application)
    {
        _application = application;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(IConsole console)
    {
        _application
            .UseResponseCompression()
            .UseProblemDetails()
            .UseHttpsRedirection()
            .UseCors("API")
            .UseAuthorization();

        _application.MapHealthChecks("/status");
        _application.MapGraphQL();
        _application.UseSwaggerInDevelopment();
        _application.MapControllers();
        await _application.RunAsync();
    }
}