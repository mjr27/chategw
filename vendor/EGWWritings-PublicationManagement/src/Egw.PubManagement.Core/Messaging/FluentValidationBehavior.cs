using FluentValidation;
using FluentValidation.Results;

using MediatR;
namespace Egw.PubManagement.Core.Messaging;

/// <inheritdoc />
public class FluentValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Default constructor
    /// </summary>
    public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var errors = new List<ValidationFailure>();
        foreach (IValidator<TRequest> validator in _validators)
        {
            ValidationResult validationResult = await validator.ValidateAsync(context, cancellationToken);
            if (!validationResult.IsValid)
            {
                errors.AddRange(validationResult.Errors);
            }
        }

        if (errors.Any())
        {
            throw new ValidationException("Validation failed", errors);
        }

        return await next();
    }
}