using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record FollowUserCommand(Guid FollowedId) : IRequest, IUserIdAssignable
{
    public Guid FollowerId { get; private set; }
    public void SetUserId(Guid userId)
    {
        FollowerId = userId;
    }
    private class FollowHandler(IUserRepository repository) : IRequestHandler<FollowUserCommand>
    {
        private readonly IUserRepository _repository = repository;
        public async Task Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            if (request.FollowerId == request.FollowedId)
                throw new BadRequestException("User can't follow himself");

            bool alreadyFollows = await _repository.IsFollowingAsync(request.FollowerId, request.FollowedId, cancellationToken);

            if (alreadyFollows)
            {
                throw new BadRequestException("User is already following this user");
            }

            var follow = new Follower()
            {
                FollowerId = request.FollowerId,
                FollowedId = request.FollowedId,
            };

            await _repository.AddAsync(follow, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
