using Microsoft.EntityFrameworkCore;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Repositories;

internal class BookRepository(BookDbContext dbContext) : IBookRepository
{
    private readonly BookDbContext _dbContext = dbContext;

    public async Task AddAsync(Book entity) => await _dbContext.AddAsync(entity);

    public IQueryable<Book> GetBySearchPhraseWithAuthors(string searchPhrase)
        => _dbContext.Books.Where(b => b.Title.Contains(searchPhrase)
                                    || b.Isbn.Contains(searchPhrase)
                                    || b.Authors.Any(a => a.Name.Contains(searchPhrase)))
        .OrderBy(b => b.Title);

    public IQueryable<Book> GetBySearchPhrase(string searchPhrase)
        => _dbContext.Books
            .Where(b => b.Title.Contains(searchPhrase) || b.Isbn.Contains(searchPhrase))
            .OrderBy(b => b.Title);

    public async Task<Book?> GetByIdAsync(Guid bookId, CancellationToken cancellationToken)
    {
        return await _dbContext.Books
            .IncludeDetails()
            .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
    }

    public IQueryable<Book> GetUnseenBooks(Guid userId)
    {
        return _dbContext.Books
            .Where(b => !_dbContext.UserDrawnBook
                .Any(ub => ub.BookId == b.Id && ub.UserId == userId && ub.IsRead));
    }

    public async Task<IEnumerable<Book>> GetUserBooksAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Users
            .Include(u => u.BookShelves)
                .ThenInclude(u => u.Books)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.BookShelves.SelectMany(bs => bs.Books))
            .Distinct()
            .IncludeDetails()
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<IEnumerable<Book>> GetRandomBooksAsync(IEnumerable<Guid> userBookIds, int pageSize, CancellationToken cancellationToken)
    {
        return await _dbContext.Books
            .IncludeDetails()
            .Where(b => !userBookIds.Contains(b.Id))
            .OrderBy(b => Guid.NewGuid())
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }


    public async Task<IEnumerable<Book>> GetUserRecommendedBooksAsync(Guid userId, int pageSize, CancellationToken cancellationToken)
    {
        var userBooks = await GetUserBooksAsync(userId, cancellationToken);
        var userBookIds = userBooks.Select(b => b.Id).ToHashSet();

        // Pobranie trzech najczęstszych kategorii
        var topCategories = userBooks
            .SelectMany(b => b.Genre)
            .GroupBy(g => g)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => g.Key)
            .ToList();

        List<Book> recommendedBooks = [];

        if (topCategories.Count != 0)
        {
            recommendedBooks = await _dbContext.Books
                .IncludeDetails()
                .Where(b => b.Genre.Any(g => topCategories.Contains(g)) && !userBookIds.Contains(b.Id))
                .OrderBy(b => Guid.NewGuid())
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        // Jeśli nie mamy wystarczającej liczby książek, dobieramy losowe
        if (recommendedBooks.Count < pageSize)
        {
            userBookIds = [.. userBookIds, .. recommendedBooks.Select(b => b.Id)];
            var additionalBooks = await GetRandomBooksAsync(userBookIds, pageSize - recommendedBooks.Count, cancellationToken);
            recommendedBooks.AddRange(additionalBooks);
        }

        return recommendedBooks.OrderBy(b => Guid.NewGuid());
    }

    public async Task<IEnumerable<Book>> GetUserRecommendedBooksAsync(Guid userId, BookGenre? bookGenre, string? authorName, int pageSize, CancellationToken cancellationToken)
    {
        var userBooks = await GetUserBooksAsync(userId, cancellationToken);
        var userBookIds = userBooks.Select(b => b.Id).ToHashSet();

        var recommendedBooks = await _dbContext.Books
            .IncludeDetails()
            .Where(b => (bookGenre != null && b.Genre.Any(g => g == bookGenre) 
                    || authorName != null && b.Authors.Any(a => a.Name == authorName)) 
                    && !userBookIds.Contains(b.Id))
            .OrderBy(b => Guid.NewGuid())
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Jeśli nie mamy wystarczającej liczby książek, dobieramy losowe
        if (recommendedBooks.Count < pageSize)
        {
            userBookIds = [.. userBookIds, .. recommendedBooks.Select(b => b.Id)];
            var additionalBooks = await GetRandomBooksAsync(userBookIds, pageSize - recommendedBooks.Count, cancellationToken);
            recommendedBooks.AddRange(additionalBooks);
        }

        return recommendedBooks.OrderBy(b => Guid.NewGuid());
    }

    public void Delete(Book entity) => _dbContext.Remove(entity);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}

public static class BookExtensionMethods
{
    public static IQueryable<Book> IncludeDetails(this IQueryable<Book> books) => 
        books.Include(b => b.Authors)
            .Include(b => b.PublishingHouse)
            .Include(b => b.Reviews);
}