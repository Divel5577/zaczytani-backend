using FluentValidation;
using Zaczytani.Application.Shared.Queries;

namespace Zaczytani.Application.Shared.Validators;

public class SearchBookQueryValidator : AbstractValidator<SearchBookQuery>
{
    public SearchBookQueryValidator()
    {
        RuleFor(query => query.SearchPhrase)
            .NotEmpty().WithMessage("Search phrase cannot be empty.")
            .MinimumLength(3).WithMessage("Search phrase must be at least 3 characters long.");
    }
}