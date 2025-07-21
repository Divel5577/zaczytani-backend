using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;

public class GetMyChallengesQuery : IRequest<IEnumerable<ChallengeDto>>, IUserIdAssignable
{
    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class GetMyChallengesQueryHandler(IChallengeRepository challengeRepository, IMapper mapper)
        : IRequestHandler<GetMyChallengesQuery, IEnumerable<ChallengeDto>>
    {
        private readonly IChallengeRepository _challengeRepository = challengeRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<ChallengeDto>> Handle(GetMyChallengesQuery request, CancellationToken cancellationToken)
        {
            var challenges = await _challengeRepository.GetChallenges()
                .Include(ch => ch.UserProgress)
                .Where(ch => ch.UserId == request.UserId)
                .Where(ch => !ch.UserProgress.Any(up => up.UserId == request.UserId))
                .ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<ChallengeDto>>(challenges);
        }
    }
}
