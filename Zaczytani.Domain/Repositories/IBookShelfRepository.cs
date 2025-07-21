using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;

namespace Zaczytani.Domain.Repositories;

public interface IBookShelfRepository
{
    Task<BookShelf?> GetBookShelfByTypeAsync(BookShelfType type, Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<BookShelf>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<BookShelf?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BookShelf?> GetByIdWithBooksAsync(Guid shelfId, Guid userId, CancellationToken cancellationToken);
    Task AddAsync(BookShelf bookshelf, CancellationToken cancellationToken);
    Task UpdateAsync(BookShelf bookshelf, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<int> GetBookCountOnReadShelfAsync(Guid bookId, CancellationToken cancellationToken);
    int GetBookCountOnReadShelf(Guid bookId);
    Task<IEnumerable<Book>> GetTopBooksByShelfIdAsync(Guid shelfId, int count, CancellationToken cancellationToken);
    Task<int> CountBooksAsync(Guid userId, BookShelfType type, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
