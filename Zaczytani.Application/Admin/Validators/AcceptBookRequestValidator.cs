using FluentValidation;
using Zaczytani.Application.Admin.Commands;

namespace Zaczytani.Application.Admin.Validators;

public class AcceptBookRequestValidator : AbstractValidator<CreateBookCommand>
{
    public AcceptBookRequestValidator()
    {
        Include(new CreateBookCommandValidator());
    }
}