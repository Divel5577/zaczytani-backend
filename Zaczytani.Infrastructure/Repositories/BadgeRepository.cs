using Microsoft.EntityFrameworkCore;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Repositories;

internal class BadgeRepository(BookDbContext dbContext) : IBadgeRepository
{
    private readonly BookDbContext _dbContext = dbContext;

    public async Task<IEnumerable<UserBadge>> GetUserBadgesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserBadges
            .Include(ub => ub.Badge)
            .Where(ub => ub.UserId == userId)
            .ToListAsync(cancellationToken);
    }
    public async Task<Badge?> GetBadgeDefinitionByIdAsync(Guid badgeId, CancellationToken cancellationToken)
    {
        return await _dbContext.Badges.FirstOrDefaultAsync(b => b.Id == badgeId, cancellationToken);
    }
    public async Task<Badge?> GetBadgeDefinitionByTypeAsync(BadgeType type, CancellationToken cancellationToken)
    {
        return await _dbContext.Badges.FirstOrDefaultAsync(b => b.Type == type, cancellationToken);
    }
    public async Task<IEnumerable<Badge>> GetAllBadgeDefinitionsAsync(BadgeType? type, CancellationToken cancellationToken)
    {
        var query = _dbContext.Badges.AsQueryable();

        if (type.HasValue)
            query = query.Where(b => b.Type == type.Value);

        return await query.ToListAsync(cancellationToken);
    }
    public async Task AddUserBadgeAsync(UserBadge userBadge, CancellationToken cancellationToken)
    {
        await _dbContext.UserBadges.AddAsync(userBadge, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task DeleteUserBadgeAsync(UserBadge userBadge, CancellationToken cancellationToken)
    {
        _dbContext.UserBadges.Remove(userBadge);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
