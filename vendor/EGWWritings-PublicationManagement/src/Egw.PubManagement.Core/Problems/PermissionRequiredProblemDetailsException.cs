using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Http;
namespace Egw.PubManagement.Core.Problems;

/// <summary>
/// Permission required
/// </summary>
public class PermissionRequiredProblemDetailsException : ProblemDetailsException
{
    /// <inheritdoc />
    public PermissionRequiredProblemDetailsException(string details = "Permission denied")
        : base(StatusCodes.Status403Forbidden)
    {
        Details.Detail = details;
    }
}