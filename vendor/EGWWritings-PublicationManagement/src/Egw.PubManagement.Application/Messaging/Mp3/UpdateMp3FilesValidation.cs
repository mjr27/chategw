using FluentValidation;

namespace Egw.PubManagement.Application.Messaging.Mp3;

/// <summary>
/// Validator for <see cref="UpdateMp3FilesInput"/>
/// </summary>
public class UpdateMp3FilesValidation : AbstractValidator<UpdateMp3FilesInput>
{
    /// <inheritdoc />
    public UpdateMp3FilesValidation()
    {
        RuleFor(r => r.PublicationId)
            .GreaterThan(0);
        RuleFor(r => r.Mp3Files)
            .NotEmpty();
        RuleForEach(r => r.Mp3Files)
            .ChildRules(r => r.RuleFor(f => f.ParaId)
                .NotEmpty())
            .ChildRules(r =>
                r.RuleFor(f => f.File)
                    .NotEmpty()
                    .Must(fn => !string.IsNullOrEmpty(fn.FileName))
                    .WithMessage(fn => $"Incorrect MP3 file name format: {fn.File}")
            );
    }
}