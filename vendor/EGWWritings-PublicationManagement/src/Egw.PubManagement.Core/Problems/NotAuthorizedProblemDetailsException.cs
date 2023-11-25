using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Http;
namespace Egw.PubManagement.Core.Problems;

/// <summary>
/// Not authorized Exception
/// </summary>
public class NotAuthorizedProblemDetailsException : ProblemDetailsException
{
    /// <inheritdoc />
    public NotAuthorizedProblemDetailsException(string details = "Not Authorized") : base(StatusCodes.Status401Unauthorized)
    {
        Details.Detail = details;
    }
}