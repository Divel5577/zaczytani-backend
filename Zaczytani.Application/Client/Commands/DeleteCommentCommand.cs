using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record DeleteCommentCommand(Guid CommentId) : IRequest, IUserIdAssignable
{
    public Guid UserId { get; private set; }
    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }
    private class Handler(IReviewRepository repository) : IRequestHandler<DeleteCommentCommand>
    {
        private readonly IReviewRepository _repository = repository;
        public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _repository.GetCommentByIdAsync(request.CommentId, cancellationToken);
            if (comment == null || comment.UserId != request.UserId)
                throw new NotFoundException("Comment not found or you do not have access to it.");
            await _repository.DeleteCommentAsync(request.CommentId, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

        }
    }
}

