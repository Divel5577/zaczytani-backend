using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

public record UnlikeReviewCommand(Guid ReviewId) : IRequest, IUserIdAssignable
{
    private Guid UserId { get; set; }

    public void SetUserId(Guid userId) => UserId = userId;

    private class Handler(IReviewRepository reviewRepository) : IRequestHandler<UnlikeReviewCommand>
    {
        private readonly IReviewRepository _reviewRepository = reviewRepository;

        public async Task Handle(UnlikeReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(request.ReviewId, cancellationToken)
                ?? throw new NotFoundException("Review not found");

            if (!review.Likes.Contains(request.UserId))
                throw new InvalidOperationException("User has not liked this review.");

            review.Likes.Remove(request.UserId);

            await _reviewRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
