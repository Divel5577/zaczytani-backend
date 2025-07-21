using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;

public record GetUserBadgesQuery(Guid UserId) : IRequest<IEnumerable<BadgeDto>>;

internal class GetUserBadgesQueryHandler(IBadgeRepository badgeRepository) : IRequestHandler<GetUserBadgesQuery, IEnumerable<BadgeDto>>
{
    public async Task<IEnumerable<BadgeDto>> Handle(GetUserBadgesQuery request, CancellationToken cancellationToken)
    {
        var userBadges = await badgeRepository.GetUserBadgesAsync(request.UserId, cancellationToken);

        return userBadges.Select(ub => new BadgeDto
        {
            Id = ub.Badge.Id,
            Name = ub.Badge.Name,
            IconUrl = ub.Badge.IconPath,
            Type = ub.Badge.Type.ToString()
        });
    }
}
