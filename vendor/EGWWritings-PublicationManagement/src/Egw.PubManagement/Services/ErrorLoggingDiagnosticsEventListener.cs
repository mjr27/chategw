using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Execution.Processing;
using HotChocolate.Resolvers;
namespace Egw.PubManagement.Services;

/// <summary>
/// GraphQL error log reporter
/// </summary>
public class ErrorLoggingDiagnosticsEventListener : ExecutionDiagnosticEventListener
{
    private readonly ILogger<ErrorLoggingDiagnosticsEventListener> _logger;

    /// <summary>
    /// Default constructor
    /// </summary>
    public ErrorLoggingDiagnosticsEventListener(
        ILogger<ErrorLoggingDiagnosticsEventListener> logger)
    {
        this._logger = logger;
    }

    /// <inheritdoc />
    public override void ResolverError(
        IMiddlewareContext context,
        IError error)
    {
        _logger.LogError(error.Exception, "{Message}", error.Message);
    }

    /// <inheritdoc />
    public override void TaskError(
        IExecutionTask task,
        IError error)
    {
        _logger.LogError(error.Exception, "{Message}", error.Message);
    }

    /// <inheritdoc />
    public override void RequestError(
        IRequestContext context,
        Exception exception)
    {
        _logger.LogError(exception, "{Message}", "RequestError");
    }

    /// <inheritdoc />
    public override void SubscriptionEventError(
        SubscriptionEventContext context,
        Exception exception)
    {
        _logger.LogError(exception, "{Message}", "SubscriptionEventError");
    }

    /// <inheritdoc />
    public override void SubscriptionTransportError(
        ISubscription subscription,
        Exception exception)
    {
        _logger.LogError(exception, "{Message}", "SubscriptionTransportError");
    }
}