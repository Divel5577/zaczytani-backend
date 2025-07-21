using AutoMapper;
using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Helpers;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public class CreateChallengeCommand : IRequest, IUserIdAssignable
{
    public int BooksToRead { get; set; }
    public ChallengeType Critiera { get; set; }
    public string? CriteriaValue { get; set; } = null!;

    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class CreateChallengeCommandHandler(IChallengeRepository challengeRepository, IMapper mapper) : IRequestHandler<CreateChallengeCommand>
    {
        private readonly IChallengeRepository _challengeRepository = challengeRepository;
        private readonly IMapper _mapper = mapper;

        public async Task Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
        {
            var challenge = _mapper.Map<Challenge>(request);
            challenge.UserId = request.UserId;
            challenge.Criteria = request.Critiera;

            if (challenge.Criteria == ChallengeType.Genre)
            {
                try
                {
                    challenge.CriteriaValue = EnumHelper.GetEnumValueFromDescription<BookGenre>(challenge.CriteriaValue!).ToString();
                }
                catch (ArgumentException)
                {
                    throw new BadRequestException("Provided criteria value is invalid");
                }
            }

            await _challengeRepository.AddAsync(challenge, cancellationToken);
            await _challengeRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
