using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Zaczytani.Application.Client.Commands;

public record DetachFromChallengeCommand(Guid ChallengeId) : IRequest, IUserIdAssignable
{
    public Guid UserId { get; private set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class Handler(IChallengeRepository challengeRepository) : IRequestHandler<DetachFromChallengeCommand>
    {
        private readonly IChallengeRepository _challengeRepository = challengeRepository;

        public async Task Handle(DetachFromChallengeCommand request, CancellationToken cancellationToken)
        {
            var progresses = await _challengeRepository.GetChallengesWithProgressByUserId(request.UserId, cancellationToken);
            var progress = progresses.FirstOrDefault(p => p.ChallengeId == request.ChallengeId);

            if (progress is null)
            {
                throw new NotFoundException("You are not part of this challenge.");
            }

            await _challengeRepository.DeleteProgressAsync(progress.Id, cancellationToken);
            await _challengeRepository.SaveChangesAsync(cancellationToken);

            var challenge = await _challengeRepository.GetChallenge(request.ChallengeId, cancellationToken);

            if (challenge is null || challenge.UserId == Guid.Empty)
            {
                var remainingParticipants = await _challengeRepository
                    .GetChallengesWithProgressByChallengeId(request.ChallengeId, cancellationToken);

                if (!remainingParticipants.Any())
                {
                    await _challengeRepository.DeleteAsync(request.ChallengeId, cancellationToken);
                    await _challengeRepository.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
