using Zaczytani.Domain.Entities;

namespace Zaczytani.Domain.Repositories;

public interface IUserDrawnBookRepository
{
    Task<UserDrawnBook?> GetDrawnBookByUserIdAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken);
    Task AddAsync(UserDrawnBook userDrawnBook, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
