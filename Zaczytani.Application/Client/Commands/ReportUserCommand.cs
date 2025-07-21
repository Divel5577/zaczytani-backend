using AutoMapper;
using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public class ReportUserCommand : IRequest, IUserIdAssignable
{
    public string Content { get; set; } = null!;

    public ReportCategory Category { get; set; }

    private Guid ReviewId { get; set; }

    private Guid UserId { get; set; }

    public void SetUserId(Guid userId) => UserId = userId;

    public void SetReviewId(Guid reviewId) => ReviewId = reviewId;

    private class ReportUserCommandHandler(IReportRepository reportRepository, IReviewRepository reviewRepository, IMapper mapper) : IRequestHandler<ReportUserCommand>
    {
        private readonly IReviewRepository _reviewRepository = reviewRepository;
        private readonly IReportRepository _reportRepository = reportRepository;
        private readonly IMapper _mapper = mapper;
        public async Task Handle(ReportUserCommand request, CancellationToken cancellationToken)
        {
            _ = await _reviewRepository.GetReviewByIdAsync(request.ReviewId, cancellationToken)
                ?? throw new NotFoundException("Review with given ID not found");

            var report = _mapper.Map<Report>(request);
            report.UserId = request.UserId;
            report.ReviewId = request.ReviewId;

            await _reportRepository.AddAsync(report, cancellationToken);
            await _reportRepository.SaveChangesAsync();
        }
    }
}
