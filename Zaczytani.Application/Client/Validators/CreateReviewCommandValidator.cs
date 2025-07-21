using FluentValidation;
using Zaczytani.Application.Client.Commands;

namespace Zaczytani.Application.Client.Validators;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.Progress)
            .NotEmpty().WithMessage("Progress must be specified.");
    }
}
