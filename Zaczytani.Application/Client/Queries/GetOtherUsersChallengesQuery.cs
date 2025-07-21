using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Helpers;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;

public class GetOtherUsersChallengesQuery : IRequest<IEnumerable<ChallengeDto>>, IUserIdAssignable
{
    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class GetAllChallengesQueryHandler(IChallengeRepository challengeRepository, IMapper mapper) : IRequestHandler<GetOtherUsersChallengesQuery, IEnumerable<ChallengeDto>>
    {
        private readonly IChallengeRepository _challengeRepository = challengeRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<ChallengeDto>> Handle(GetOtherUsersChallengesQuery request, CancellationToken cancellationToken)
        {
            var challenges = await _challengeRepository.GetChallenges()
                .Include(ch => ch.UserProgress)
                .Where(ch => ch.UserId != request.UserId)
                .Where(ch => !ch.UserProgress.Any(up => up.UserId == request.UserId))
                .ToListAsync(cancellationToken);

            var challengeDtos = _mapper.Map<IEnumerable<ChallengeDto>>(challenges);

            foreach (var dto in challengeDtos)
            {

                if (dto.Criteria == ChallengeType.Genre)
                {
                    if (Enum.TryParse(dto.CriteriaValue, false, out BookGenre bookGenre))
                    {
                        dto.CriteriaValue = EnumHelper.GetEnumDescription(bookGenre);
                    }
                    else
                    {
                        throw new BadRequestException("Provided criteria value is invalid");
                    }

                }
            }

            return challengeDtos;
        }
    }
}
