using FluentValidation;
using Zaczytani.Application.Admin.Commands;

namespace Zaczytani.Application.Admin.Validators;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Book title is required.")
            .MinimumLength(2).WithMessage("Book title must be at least 2 characters long.")
            .MaximumLength(150).WithMessage("Book title cannot exceed 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Isbn)
            .NotEmpty().WithMessage("ISBN is required.")
            .Matches(@"^\d{10}(\d{3})?$").WithMessage("ISBN must be 10 or 13 digits.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Page number must be less than or equal to 10,000.");

        RuleFor(x => x.ReleaseDate)
            .NotEmpty().WithMessage("Release date is required.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Release date cannot be in the future.");

        RuleFor(x => x.Authors)
            .NotNull().WithMessage("Authors list cannot be null.")
            .Must(authors => authors.Count > 0).WithMessage("At least one author is required.")
            .ForEach(author =>
            {
                author.NotEmpty().WithMessage("Author cannot be null.");
            });

        RuleFor(x => x.PublishingHouse)
            .NotEmpty().WithMessage("Publishing house is required.");

        RuleFor(x => x.Genre)
            .NotNull().WithMessage("Genre list cannot be null.")
            .Must(genres => genres.Count > 0).WithMessage("At least one genre is required.")
            .ForEach(genre =>
            {
                genre.IsInEnum().WithMessage("Invalid genre type.");
            });

        RuleFor(x => x.Series)
          .MaximumLength(200).WithMessage("Series name cannot exceed 200 characters.");
    }
}
