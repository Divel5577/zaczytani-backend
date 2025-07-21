using FluentValidation;
using Zaczytani.Application.Admin.Commands;

namespace Zaczytani.Application.Admin.Validators;

public class ProcessReportCommandValidator : AbstractValidator<ProcessReportCommand>
{
    public ProcessReportCommandValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status selected.");

        RuleFor(x => x.ReportId)
            .NotEmpty().WithMessage("ReportId cannot be empty.")
            .Must(id => id != Guid.Empty).WithMessage("ReportId must be a valid GUID.");
    }
}
