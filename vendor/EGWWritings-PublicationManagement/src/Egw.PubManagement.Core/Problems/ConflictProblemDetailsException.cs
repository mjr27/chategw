using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Http;
namespace Egw.PubManagement.Core.Problems;

/// <summary>
/// Conflict (409) exception
/// </summary>
public class ConflictProblemDetailsException : ProblemDetailsException
{
    /// <inheritdoc />
    public ConflictProblemDetailsException(string details = "Conflict") : base(StatusCodes.Status409Conflict)
    {
        Details.Detail = details;
    }
}