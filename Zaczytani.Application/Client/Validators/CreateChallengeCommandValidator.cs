using FluentValidation;
using Zaczytani.Application.Client.Commands;
using Zaczytani.Domain.Enums;

namespace Zaczytani.Application.Client.Validators;

public class CreateChallengeCommandValidator : AbstractValidator<CreateChallengeCommand>
{
    public CreateChallengeCommandValidator()
    {
        RuleFor(x => x.BooksToRead)
            .NotNull()
            .WithMessage("BooksToRead must not be null.");

        RuleFor(x => x.Critiera)
            .NotNull()
            .WithMessage("Criteria must not be null.");

        RuleFor(x => x.CriteriaValue)
            .Must((command, criteriaValue) =>
                command.Critiera == ChallengeType.BooksCount ? criteriaValue == null : !string.IsNullOrWhiteSpace(criteriaValue))
            .WithMessage("CriteriaValue must be null when Criteria is 'BooksCount', otherwise it must be provided.");
    }
}