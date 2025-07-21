using AutoMapper;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Helpers;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;

public class GetChallengeProgressesQuery : IRequest<IEnumerable<ChallengeProgressDto>>, IUserIdAssignable
{
    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class GetChallengeProgressesQueryHandler(IChallengeRepository challengeRepository, IMapper mapper) : IRequestHandler<GetChallengeProgressesQuery, IEnumerable<ChallengeProgressDto>>
    {
        private readonly IChallengeRepository _challengeRepository = challengeRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<IEnumerable<ChallengeProgressDto>> Handle(GetChallengeProgressesQuery request, CancellationToken cancellationToken)
        {
            var progresses = await _challengeRepository.GetChallengesWithProgressByUserId(request.UserId, cancellationToken);
            var progresseDtos = _mapper.Map<IEnumerable<ChallengeProgressDto>>(progresses);

            foreach (var dto in progresseDtos)
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

            return progresseDtos;
        }
    }
}