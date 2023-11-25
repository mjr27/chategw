using Egw.PubManagement.Core.Problems;

using FluentValidation;

using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Http;
namespace Egw.PubManagement.Core.GraphQl;

/// <summary>
/// Problem details error
/// </summary>
public class ProblemDetailsError
{
    /// <summary>
    /// Error group
    /// </summary>
    public class ErrorGroup
    {
        /// <summary>
        /// Field
        /// </summary>
        public required string Field { get; init; }

        /// <summary>
        /// Errors
        /// </summary>
        public required string[] Errors { get; init; }
    }

    private ProblemDetailsError(ProblemDetailsException ex) : this(
        ex.Details.Status ?? StatusCodes.Status400BadRequest,
        ex.Details.Detail ?? ex.Details.Title ?? ex.Message,
        null)
    {
    }

    private ProblemDetailsError(int code, string message, string? details,
        IReadOnlyCollection<ErrorGroup>? errors = null)
    {
        Code = code;
        Message = message;
        Details = details;
        Errors = errors;
    }

    /// <summary>
    /// HTTP-based status code
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Exception message
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Exception details
    /// </summary>
    public string? Details { get; }


    /// <summary>
    /// Custom errors
    /// </summary>
    public IReadOnlyCollection<ErrorGroup>? Errors { get; }

    /// <summary>
    /// Conflict
    /// </summary>
    public static ProblemDetailsError CreateErrorFrom(ConflictProblemDetailsException ex) => new(ex);

    /// <summary>
    /// Not Authorized
    /// </summary>
    public static ProblemDetailsError CreateErrorFrom(NotAuthorizedProblemDetailsException ex) => new(ex);

    /// <summary>
    /// Not Found
    /// </summary>
    public static ProblemDetailsError CreateErrorFrom(NotFoundProblemDetailsException ex) => new(ex);

    /// <summary>
    /// Permission required
    /// </summary>
    public static ProblemDetailsError CreateErrorFrom(PermissionRequiredProblemDetailsException ex) => new(ex);

    /// <summary>
    /// Validation error
    /// </summary>
    public static ProblemDetailsError CreateErrorFrom(ValidationProblemDetailsException ex) => new(ex);

    /// <summary>
    /// Validation error
    /// </summary>
    public static ProblemDetailsError CreateErrorFrom(ValidationException ex)
    {
        IEnumerable<ErrorGroup> errors = ex.Errors
            .Where(e => e is not null)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) =>
                    new ErrorGroup { Field = propertyName, Errors = errorMessages.Distinct().ToArray() });
        return new ProblemDetailsError(
            StatusCodes.Status400BadRequest,
            "Validation error",
            ex.Message,
            errors.ToArray()
        );
    }
}