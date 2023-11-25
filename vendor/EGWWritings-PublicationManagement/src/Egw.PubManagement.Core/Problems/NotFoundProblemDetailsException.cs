using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Http;
namespace Egw.PubManagement.Core.Problems;

/// <summary>
/// Not found (404) exception
/// </summary>
public class NotFoundProblemDetailsException : ProblemDetailsException
{
    /// <inheritdoc />
    public NotFoundProblemDetailsException(string details = "Object not found")
        : base(StatusCodes.Status404NotFound)
    {
        Details.Detail = details;
    }
}