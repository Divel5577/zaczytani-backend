using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

public record LikeReviewCommand(Guid ReviewId) : IRequest, IUserIdAssignable
{
    private Guid UserId { get; set; }

    public void SetUserId(Guid userId) => UserId = userId;

    private class Handler(IReviewRepository reviewRepository) : IRequestHandler<LikeReviewCommand>
    {
        private readonly IReviewRepository _reviewRepository = reviewRepository;

        public async Task Handle(LikeReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(request.ReviewId, cancellationToken)
                ?? throw new NotFoundException("Review not found");

            if (review.Likes.Contains(request.UserId))
                throw new InvalidOperationException("User has already liked this review.");

            review.Likes.Add(request.UserId);

            await _reviewRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
