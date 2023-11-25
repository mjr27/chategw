using Egw.PubManagement.Core.Problems;

using FluentValidation;

using Hellang.Middleware.ProblemDetails;

using HotChocolate;

using Microsoft.AspNetCore.Mvc;
namespace Egw.PubManagement.Core.GraphQl;

/// <inheritdoc />
public class ProblemErrorFilter : IErrorFilter
{
    /// <inheritdoc />
    public IError OnError(IError error)
    {
        Exception? exception = error.Exception;
        return exception switch
        {
            ProblemDetailsException e => BuildProblemException(error, e),
            ValidationException e => BuildValidationException(error, e),
            _ => error
        };
    }

    private IError BuildValidationException(
        IError error,
        ValidationException validationException)
    {
        var errorDictionary = validationException.Errors
            .Where(e => e is not null)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName, Values = errorMessages.Distinct().ToArray()
                })
            .ToDictionary(x => x.Key, x => x.Values);
        return BuildProblemException(error, new ValidationProblemDetailsException(errorDictionary));
    }

    private IError BuildProblemException(IError error, ProblemDetailsException exception)
    {
        IErrorBuilder errorBuilder = ErrorBuilder.New()
            .SetCode(exception.Details.Status?.ToString())
            .SetException(exception)
            .SetMessage(exception.Details.Detail ?? exception.Message)
            .SetPath(error.Path)
            .SetExtension("problem", ToProblem(exception.Details));
        if (error.Locations is not null)
        {
            foreach (Location loc in error.Locations)
            {
                errorBuilder = errorBuilder.AddLocation(loc);
            }
        }

        if (exception.Details is ValidationProblemDetails vpd)
        {
            errorBuilder = errorBuilder.SetExtension("validation", ToErrorList(vpd));
        }

        return errorBuilder.Build();
    }

    private IReadOnlyDictionary<string, object?> ToErrorList(ValidationProblemDetails problem)
    {
        var result = new Dictionary<string, object?>();
        foreach (KeyValuePair<string, string[]> error in problem.Errors)
        {
            result[error.Key] = error.Value;
        }

        return result;
    }

    private IReadOnlyDictionary<string, object?> ToProblem(ProblemDetails problem)
    {
        var result = new Dictionary<string, object?>
        {
            ["detail"] = problem.Detail,
            ["instance"] = problem.Instance,
            ["status"] = problem.Status,
            ["title"] = problem.Title,
            ["type"] = problem.Type
        };
        return result;
    }
}