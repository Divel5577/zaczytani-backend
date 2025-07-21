using AutoMapper;
using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Application.Services;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record CreateCommentCommand(string Content) : IRequest, IUserIdAssignable
{
    private Guid ReviewId { get; set; }
    public void SetReviewId(Guid reviewId) => ReviewId = reviewId;

    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class CreateCommentCommandHandler(IReviewRepository reviewRepository, IMapper mapper, IBadgeAssignerService badgeAssignerService, IMediator mediator) : IRequestHandler<CreateCommentCommand>
    {
        private readonly IReviewRepository _reviewRepository = reviewRepository;
        private readonly IBadgeAssignerService _badgeAssignerService = badgeAssignerService;
        private readonly IMapper _mapper = mapper;
        private readonly IMediator _mediator = mediator;

        public async Task Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            _ = await _reviewRepository.GetReviewByIdAsync(request.ReviewId, cancellationToken)
                ?? throw new NotFoundException("Review with given ID not found");

            var comment = _mapper.Map<Comment>(request);
            comment.ReviewId = request.ReviewId;
            comment.UserId = request.UserId;

            await _reviewRepository.AddCommentAsync(comment, cancellationToken);
            await _reviewRepository.SaveChangesAsync(cancellationToken);

            _badgeAssignerService.SetUserId(request.UserId);
            var count = await _reviewRepository.CountUserCommentsAsync(request.UserId, cancellationToken);
            await _badgeAssignerService.CheckCommentBadgesAsync(count, cancellationToken);

            await _mediator.Send(new CommentCreatedCommand(request.ReviewId), cancellationToken);
        }
    }
}
