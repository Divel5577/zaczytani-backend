using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record JoinChallengeCommand(Guid ChallengeId) : IRequest, IUserIdAssignable
{
    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class JoinChallengeCommandHandler(IChallengeRepository challengeRepository) : IRequestHandler<JoinChallengeCommand>
    {
        private readonly IChallengeRepository _challengeRepository = challengeRepository;
        public async Task Handle(JoinChallengeCommand request, CancellationToken cancellationToken)
        {
            var challenge = await _challengeRepository.GetChallenge(request.ChallengeId, cancellationToken)
                ?? throw new NotFoundException("Challenge with given ID not found");

            var progress = new ChallengeProgress()
            {
                ChallengeId = challenge.Id,
                UserId = request.UserId,
            };

            await _challengeRepository.AddAsync(progress, cancellationToken);
            await _challengeRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
