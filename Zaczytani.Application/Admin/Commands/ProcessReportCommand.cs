using MediatR;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Application.Shared.Commands;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Commands;

public record ProcessReportCommand(Guid ReportId, ReportStatus Status) : IRequest
{
    private class ProcessReportCommandHandler(IReportRepository reportRepository, IMediator mediator) : IRequestHandler<ProcessReportCommand>
    {
        private readonly IReportRepository _reportRepository = reportRepository;
        private readonly IMediator _mediator = mediator;

        public async Task Handle(ProcessReportCommand request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetReportById(request.ReportId)
                .Include(r => r.Review)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Report with given ID not found");

            report.Status = request.Status;

            if (request.Status == ReportStatus.Blocked)
            {
                await _mediator.Send(new BlockUserCommand(report.Review.UserId,report.Id), cancellationToken);
            }

            await _reportRepository.SaveChangesAsync();
        }
    }
}
