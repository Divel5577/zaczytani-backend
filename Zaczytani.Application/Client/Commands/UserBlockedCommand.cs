using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Helpers;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record UserBlockedCommand(Guid ReportId):IRequest
{
    private class UserBlockedCommandHandler(IEmailInfoRepository emailInfoRepository, IReportRepository reportRepository) : IRequestHandler<UserBlockedCommand>
    {
        private readonly IReportRepository _reportRepository = reportRepository;
        private readonly IEmailInfoRepository _emailInfoRepository = emailInfoRepository;
      
        public async Task Handle(UserBlockedCommand request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetReportById(request.ReportId)
                .Include(r => r.Review)
                .Include(r => r.User)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Report with given ID not found");

            if (report.Review.User.Email is null)
                return;
            
            var enumDescription = EnumHelper.GetEnumDescription(report.Category);

            var emailInfo = new EmailInfo()
            {
                EmailTo = report.Review.User.Email,
                EmailContent = [enumDescription,report.Review.User.FirstName,report.Review.User.LastName],
                EmailTemplate = EmailTemplate.UserBlocked,
            };

            await _emailInfoRepository.AddAsync(emailInfo, cancellationToken);
            await _emailInfoRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
