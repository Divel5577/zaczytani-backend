using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record UnFollowUserCommand(Guid FollowedId) : IRequest, IUserIdAssignable
{
    public Guid FollowerId { get; private set; }
    public void SetUserId(Guid userId)
    {
        FollowerId = userId;
    }
    private class UnFollowHandler(IUserRepository repository) : IRequestHandler<UnFollowUserCommand>
    {
        private readonly IUserRepository _repository = repository;
        public async Task Handle(UnFollowUserCommand request, CancellationToken cancellationToken)
        {
            if (request.FollowerId == request.FollowedId)
                throw new BadRequestException("User can't unfollow himself");
            var follow = await _repository.GetFollowAsync(request.FollowerId,request.FollowedId, cancellationToken);
            if (follow == null)
                throw new NotFoundException("Follow not found or you do not have access to it.");
            await _repository.DeleteFollowAsync(request.FollowerId,request.FollowedId, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}