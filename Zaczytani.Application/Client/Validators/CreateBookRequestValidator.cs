using FluentValidation;
using Zaczytani.Application.Client.Commands;

namespace Zaczytani.Application.Client.Validators;

public class CreateBookRequestCommandValidator : AbstractValidator<CreateBookRequestCommand>
{
    public CreateBookRequestCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Book title is required.")
            .MinimumLength(2).WithMessage("Book title must be at least 2 characters long.")
            .MaximumLength(150).WithMessage("Book title cannot exceed 150 characters.");

        RuleFor(x => x.Description)
            .MinimumLength(10).When(x => x.Description is not null).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Isbn)
            .Matches(@"^\d{10}(\d{3})?$").When(x => x.Isbn is not null).WithMessage("ISBN must be 10 or 13 digits.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).When(x => x.PageNumber is not null).WithMessage("Page number must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Page number must be less than or equal to 10,000.");

        RuleFor(x => x.ReleaseDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).When(x => x.ReleaseDate.HasValue).WithMessage("Release date cannot be in the future.");

        RuleFor(x => x.Authors)
            .NotEmpty().WithMessage("Authors are required.");

        RuleFor(x => x.Genre)
            .ForEach(genre =>
            {
                genre.IsInEnum().WithMessage("Invalid book genre provided.");
            });

        RuleFor(x => x.Genre)
            .Must(genres => genres.Distinct().Count() == genres.Count())
            .WithMessage("The 'Genre' field contains duplicate values.");

        RuleFor(x => x.Series)
            .MaximumLength(150).WithMessage("Book series cannot exceed 150 characters.");
    }
}