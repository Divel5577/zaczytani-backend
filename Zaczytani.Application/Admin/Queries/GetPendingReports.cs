using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Queries;

public class GetPendingReports : IRequest<IEnumerable<ReportDto>>
{
    private class GetPendingReportsHandler(IReportRepository reportRepository, IMapper mapper) : IRequestHandler<GetPendingReports, IEnumerable<ReportDto>>
    {
        private readonly IReportRepository _reportRepository = reportRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<IEnumerable<ReportDto>> Handle(GetPendingReports request, CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetPendingReports()
                .Include(r => r.User)
                .Include(r => r.Review)
                    .ThenInclude(r => r.User)
                .ToListAsync(cancellationToken);

            var reportDtos = _mapper.Map<IEnumerable<ReportDto>>(reports);

            return reportDtos;
        }
    }
}
