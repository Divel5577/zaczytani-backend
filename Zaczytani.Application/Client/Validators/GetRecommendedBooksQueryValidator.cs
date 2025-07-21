using FluentValidation;
using Zaczytani.Application.Client.Queries;

namespace Zaczytani.Application.Client.Validators;

public class GetRecommendedBooksQueryValidator : AbstractValidator<GetRecommendedBooksQuery>
{
    public GetRecommendedBooksQueryValidator()
    {
        RuleFor(x => x.PageSize)
            .NotEmpty().WithMessage("PageSize is required.")
            .LessThanOrEqualTo(100).WithMessage("Maximum page size is 100");

        RuleFor(x => x.AuthorName)
            .MaximumLength(100).When(x => x.AuthorName is not null).WithMessage("Author name cannot exceed 100 characters.");
    }
}