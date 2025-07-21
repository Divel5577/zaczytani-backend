using Microsoft.EntityFrameworkCore;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Repositories;
internal class BookShelfRepository(BookDbContext dbContext) : IBookShelfRepository
{
    private readonly BookDbContext _dbContext = dbContext;

    public async Task<BookShelf?> GetBookShelfByTypeAsync(BookShelfType type, Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.BookShelves
            .Include(b => b.Books)
                .ThenInclude(b => b.Authors)
            .Where(b => b.Type == type && b.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<BookShelf>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.BookShelves
            .Include(bs => bs.Books)
                .ThenInclude(b => b.Reviews)
            .Include(bs => bs.Books)
                .ThenInclude(b => b.Authors)
            .Include(bs => bs.Books)
                .ThenInclude(b => b.PublishingHouse)
            .Where(b => b.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<BookShelf?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.BookShelves
            .Include(b => b.Books)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<BookShelf?> GetByIdWithBooksAsync(Guid shelfId, Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.BookShelves
            .Include(bs => bs.Books)
                .ThenInclude(b => b.Reviews)
            .Include(bs => bs.Books)
                .ThenInclude(b => b.Authors)
            .Include(bs => bs.Books)
                .ThenInclude(b => b.PublishingHouse)
            .FirstOrDefaultAsync(bs => bs.Id == shelfId && bs.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(BookShelf bookshelf, CancellationToken cancellationToken)
    {
        await _dbContext.BookShelves.AddAsync(bookshelf, cancellationToken);
    }

    public async Task UpdateAsync(BookShelf bookshelf, CancellationToken cancellationToken)
    {
        _dbContext.BookShelves.Update(bookshelf);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var shelf = await GetByIdAsync(id, cancellationToken);
        if (shelf != null)
        {
            _dbContext.BookShelves.Remove(shelf);
        }
    }

    public async Task<int> GetBookCountOnReadShelfAsync(Guid bookId, CancellationToken cancellationToken)
    {
        return await _dbContext.BookShelves
            .Where(bs => bs.Type == BookShelfType.Read)
            .SelectMany(bs => bs.Books)
            .CountAsync(b => b.Id == bookId, cancellationToken);
    }

    public int GetBookCountOnReadShelf(Guid bookId)
    {
        return _dbContext.BookShelves
            .Where(bs => bs.Type == BookShelfType.Read)
            .SelectMany(bs => bs.Books)
            .Count(b => b.Id == bookId);
    }
    public async Task<IEnumerable<Book>> GetTopBooksByShelfIdAsync(Guid shelfId, int count, CancellationToken cancellationToken)
    {
        return await _dbContext.BookShelves
            .Where(bs => bs.Id == shelfId)
            .SelectMany(bs => bs.Books) 
            .Take(count)
            .ToListAsync(cancellationToken);
    }
    public async Task<int> CountBooksAsync(Guid userId, BookShelfType type, CancellationToken cancellationToken)
    {
        return await _dbContext.BookShelves
            .Where(bs => bs.UserId == userId && bs.Type == type)
            .SelectMany(bs => bs.Books)
            .CountAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

