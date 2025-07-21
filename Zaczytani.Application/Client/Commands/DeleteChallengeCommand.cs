using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record DeleteChallengeCommand(Guid ChallengeId) : IRequest, IUserIdAssignable
{
    public Guid UserId { get; private set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class Handler(IChallengeRepository challengeRepository) : IRequestHandler<DeleteChallengeCommand>
    {
        private readonly IChallengeRepository _challengeRepository = challengeRepository;

        public async Task Handle(DeleteChallengeCommand request, CancellationToken cancellationToken)
        {
            var challenge = await _challengeRepository.GetChallenge(request.ChallengeId, cancellationToken)
                ?? throw new NotFoundException("Challenge not found or you do not have access to it.");

            if (challenge.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You are not the owner of this challenge.");
            }

            var progresses = await _challengeRepository.GetChallengesWithProgressByUserId(request.UserId, cancellationToken);
            var hasParticipants = progresses.Any(p => p.ChallengeId == request.ChallengeId);

            if (!hasParticipants)
            {
                await _challengeRepository.DeleteAsync(request.ChallengeId, cancellationToken);
                await _challengeRepository.SaveChangesAsync(cancellationToken);
            }
            else
            {
                challenge.UserId = Guid.Empty;
                await _challengeRepository.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
