using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;

namespace Zaczytani.Domain.Repositories;

public interface IBadgeRepository
{
    Task<IEnumerable<UserBadge>> GetUserBadgesAsync(Guid userId, CancellationToken cancellationToken);
    Task<Badge?> GetBadgeDefinitionByIdAsync(Guid badgeId, CancellationToken cancellationToken);
    Task<Badge?> GetBadgeDefinitionByTypeAsync(BadgeType type, CancellationToken cancellationToken);
    Task<IEnumerable<Badge>> GetAllBadgeDefinitionsAsync(BadgeType? type, CancellationToken cancellationToken);
    Task AddUserBadgeAsync(UserBadge userBadge, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task DeleteUserBadgeAsync(UserBadge userBadge, CancellationToken cancellationToken);
}
