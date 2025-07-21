using Microsoft.EntityFrameworkCore;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Repositories;

internal class ChallengeRepository(BookDbContext dbContext) : IChallengeRepository
{
    private readonly BookDbContext _dbContext = dbContext;

    public async Task AddAsync(Challenge entity, CancellationToken cancellationToken) => await _dbContext.AddAsync(entity, cancellationToken);
    public async Task AddAsync(ChallengeProgress entity, CancellationToken cancellationToken) => await _dbContext.AddAsync(entity, cancellationToken);

    public IQueryable<Challenge> GetChallenges() => _dbContext.Challenges;

    public async Task<Challenge?> GetChallenge(Guid Id, CancellationToken cancellationToken)
        => await _dbContext.Challenges.FirstOrDefaultAsync(ch => ch.Id == Id, cancellationToken);

    public async Task<IEnumerable<ChallengeProgress>> GetChallengesWithProgressByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.ChallengeProgresses
            .Include(cp => cp.Challenge)
            .Where(cp => cp.UserId == userId)
            .ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<ChallengeProgress>> GetChallengesWithProgressByChallengeId(Guid challengeId, CancellationToken cancellationToken)
    {
        return await _dbContext.ChallengeProgresses
            .Where(p => p.ChallengeId == challengeId)
            .ToListAsync(cancellationToken);
    }
    public async Task DeleteAsync(Guid challengeId, CancellationToken cancellationToken)
    {
        await _dbContext.Challenges
            .Where(c => c.Id == challengeId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteProgressAsync(Guid progressId, CancellationToken cancellationToken)
    {
        await _dbContext.ChallengeProgresses
            .Where(p => p.Id == progressId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _dbContext.SaveChangesAsync(cancellationToken);
}
