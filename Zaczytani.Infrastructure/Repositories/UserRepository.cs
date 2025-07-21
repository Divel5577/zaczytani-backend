using Microsoft.EntityFrameworkCore;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Repositories;

internal class UserRepository(BookDbContext dbContext) : IUserRepository
{
    private readonly BookDbContext _dbContext = dbContext;

    public async Task<List<BookGenre>> GetFavoriteGenresAsync(Guid userId, CancellationToken cancellationToken)
    {
        var books = await _dbContext.BookShelves
             .Where(bs => bs.UserId == userId && bs.Type == BookShelfType.Read)
             .SelectMany(bs => bs.Books)
             .ToListAsync(cancellationToken);

        return books.SelectMany(b => b.Genre)
            .GroupBy(g => g)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => g.Key)
            .ToList();
    }

    public IQueryable<User> GetBySearchPhrase(string searchPhrase)
    {
        if (string.IsNullOrWhiteSpace(searchPhrase))
            return _dbContext.Users.Where(u => false);

        string[] keywords = searchPhrase.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        IQueryable<User> usersQuery = _dbContext.Users;

        if (keywords.Length == 1)
        {
            string keyword = keywords[0];
            usersQuery = usersQuery.Where(u =>
                u.FirstName.Contains(keyword) ||
                u.LastName.Contains(keyword));
        }
        else if (keywords.Length >= 2)
        {
            string firstName = keywords[0];
            string lastName = string.Join(" ", keywords.Skip(1));

            usersQuery = usersQuery.Where(u =>
                (u.FirstName.Contains(firstName) && u.LastName.Contains(lastName)) ||
                (u.FirstName.Contains(lastName) && u.LastName.Contains(firstName)));
        }

        return usersQuery;
    }

    public async Task AddAsync(Follower entity, CancellationToken cancellationToken) => await _dbContext.AddAsync(entity, cancellationToken);

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followedId, CancellationToken cancellationToken)
    {
        return await _dbContext.Followers
            .AnyAsync(f => f.FollowerId == followerId && f.FollowedId == followedId, cancellationToken);
    }

    public async Task<Follower?> GetFollowAsync(Guid followerId, Guid followedId, CancellationToken cancellationToken)
    {
        return await _dbContext.Followers
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId, cancellationToken);
    }

    public async Task DeleteFollowAsync(Guid FollowerId, Guid FollowedId, CancellationToken cancellationToken)
    {
        var follow = await GetFollowAsync(FollowerId, FollowedId, cancellationToken);
        if (follow != null)
        {
            _dbContext.Followers.Remove(follow);
        }
    }
    public async Task<int> GetFollowersCountAsync(Guid userId, CancellationToken cancellationToken)
    => await _dbContext.Followers.CountAsync(f => f.FollowedId == userId, cancellationToken);

    public async Task<int> GetFollowingCountAsync(Guid userId, CancellationToken cancellationToken)
        => await _dbContext.Followers.CountAsync(f => f.FollowerId == userId, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
