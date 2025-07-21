using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;

namespace Zaczytani.Domain.Repositories;

public interface IBookRepository
{
    Task AddAsync(Book entity);
    IQueryable<Book> GetBySearchPhraseWithAuthors(string searchPhrase);
    IQueryable<Book> GetBySearchPhrase(string searchPhrase);
    Task<Book?> GetByIdAsync(Guid bookId, CancellationToken cancellationToken);
    IQueryable<Book> GetUnseenBooks(Guid userId);
    Task<IEnumerable<Book>> GetUserBooksAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<Book>> GetRandomBooksAsync(IEnumerable<Guid> userBookIds, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<Book>> GetUserRecommendedBooksAsync(Guid userId, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<Book>> GetUserRecommendedBooksAsync(Guid userId, BookGenre? bookGenre, string? authorName, int pageSize, CancellationToken cancellationToken);
    void Delete(Book entity);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
