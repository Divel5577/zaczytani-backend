using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Queries;

public record GetBadgeDefinitionsQuery(BadgeType? Type = null) : IRequest<IEnumerable<BadgeDto>>;

internal class GetBadgeDefinitionsQueryHandler(IBadgeRepository badgeRepository)
    : IRequestHandler<GetBadgeDefinitionsQuery, IEnumerable<BadgeDto>>
{
    public async Task<IEnumerable<BadgeDto>> Handle(GetBadgeDefinitionsQuery request, CancellationToken cancellationToken)
    {
        var all = await badgeRepository.GetAllBadgeDefinitionsAsync(request.Type, cancellationToken);

        return all.Select(b => new BadgeDto
        {
            Id = b.Id,
            Name = b.Name,
            Type = b.Type.ToString(),
            IconUrl = b.IconPath
        });
    }

}
