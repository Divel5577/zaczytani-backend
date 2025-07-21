using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Repositories;
using Zaczytani.Domain.Enums;
using Zaczytani.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Zaczytani.Infrastructure.Repositories;

internal class BookRequestRepository(BookDbContext dbContext) : IBookRequestRepository
{
    private readonly BookDbContext _dbContext = dbContext;

    public async Task AddAsync(BookRequest entity) => await _dbContext.AddAsync(entity);

    public IQueryable<BookRequest> GetAllPending() => _dbContext.BookRequests
        .Where(b => b.Status == BookRequestStatus.Pending)
        .OrderBy(b => b.CreatedDate);

    public IQueryable<BookRequest> GetByUserId(Guid userId) => _dbContext.BookRequests
        .Where(b => b.UserId == userId)
        .OrderBy(b => b.CreatedDate);

    public async Task<BookRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) 
        => await _dbContext.BookRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task DeleteBookRequest(Guid id, CancellationToken cancellationToken)
    {
        var bookRequest = await GetByIdAsync(id, cancellationToken);
        if(bookRequest != null)
        {
            _dbContext.BookRequests.Remove(bookRequest);
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);
}
