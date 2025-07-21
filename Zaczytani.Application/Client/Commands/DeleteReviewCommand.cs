using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record DeleteReviewCommand(Guid ReviewId) : IRequest, IUserIdAssignable
{
    public Guid UserId { get; private set; }
    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }
    private class Handler(IReviewRepository repository) : IRequestHandler<DeleteReviewCommand>
    {
        private readonly IReviewRepository _repository = repository;
        public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _repository.GetReviewByIdAsync(request.ReviewId, cancellationToken);
            if (review == null || review.UserId != request.UserId)
                throw new NotFoundException("Review not found or you do not have access to it.");
            await _repository.DeleteAsync(request.ReviewId, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
