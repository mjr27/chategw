using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace Egw.PubManagement.Core.Problems;

/// <summary>
/// Validation problem occured
/// </summary>
public class ValidationProblemDetailsException : ProblemDetailsException
{
    /// <inheritdoc />
    public ValidationProblemDetailsException(string message, ModelStateDictionary modelState) : base(
        new ValidationProblemDetails(modelState) { Status = StatusCodes.Status400BadRequest, Detail = message })
    {
    }

    /// <inheritdoc />
    public ValidationProblemDetailsException(string fieldName, string message) : this(
        MakeModelStateDictionary(fieldName, message))
    {
    }


    /// <inheritdoc />
    public ValidationProblemDetailsException(ModelStateDictionary modelState) : this("Bad Request", modelState)
    {
    }

    /// <inheritdoc />
    public ValidationProblemDetailsException(Dictionary<string, string[]> errors) : this("Bad Request",
        MakeModelStateDictionary(errors))
    {
    }

    /// <inheritdoc />
    public ValidationProblemDetailsException(string message) : this(message, new ModelStateDictionary())
    {
    }


    private static ModelStateDictionary MakeModelStateDictionary(string fieldName, string message)
    {
        var msd = new ModelStateDictionary();
        msd.AddModelError(fieldName, message);
        return msd;
    }

    private static ModelStateDictionary MakeModelStateDictionary(IReadOnlyDictionary<string, string[]> errors)
    {
        var msd = new ModelStateDictionary();
        foreach ((string fieldName, string[] errorList) in errors)
        {
            foreach (string message in errorList)
            {
                msd.AddModelError(fieldName, message);
            }
        }

        return msd;
    }
}