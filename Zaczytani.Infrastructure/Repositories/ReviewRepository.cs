using Microsoft.EntityFrameworkCore;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Repositories;

internal class ReviewRepository(BookDbContext dbContext) : IReviewRepository
{
    private readonly BookDbContext _dbContext = dbContext;

    public async Task AddAsync(Review entity, CancellationToken cancellationToken) => await _dbContext.AddAsync(entity, cancellationToken);

    public async Task AddCommentAsync(Comment entity, CancellationToken cancellationToken) => await _dbContext.AddAsync(entity, cancellationToken);

    public async Task<IEnumerable<Review>> GetFinalReviewsByBookId(Guid bookId, CancellationToken cancellationToken) 
        => await _dbContext.Reviews
        .Include(r => r.User)
        .Include(r => r.Comments)
        .Where(r => r.BookId == bookId && r.IsFinal == true)
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Review>> GetReviewsByBookIdAndUserId(Guid bookId, Guid userId, CancellationToken cancellationToken)
        => await _dbContext.Reviews
        .Where(r => r.BookId == bookId 
            && r.UserId == userId
            && r.IsFinal == false)
        .OrderByDescending(r => r.CreatedDate)
        .ToListAsync(cancellationToken);

    public async Task<Review?> GetLatestReviewByBookIdAsync(Guid bookId, Guid userId, CancellationToken cancellationToken)
        => await _dbContext.Reviews
        .Include(r => r.Book)
        .Where(r => r.BookId == bookId && r.UserId == userId)
        .OrderByDescending(r => r.CreatedDate)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<Review?> GetReviewByIdAsync(Guid id, CancellationToken cancellationToken) 
        => await _dbContext.Reviews
        .Include(r => r.User)
        .Include(r => r.Book)
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Review?> GetFinalReviewByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _dbContext.Reviews
        .Include(r => r.Book)
            .ThenInclude(b => b.Authors)
        .Include(r => r.User)
        .Include(r => r.Comments)
            .ThenInclude(c => c.User)
        .Where(r => r.IsFinal == true)
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
   
    public async Task<Comment?> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken)
       => await _dbContext.Comments
       .Include(r => r.User)
       .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<int> CountUserReviewsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Reviews
            .CountAsync(r => r.UserId == userId, cancellationToken);
    }
    public async Task<int> CountUserCommentsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Comments.CountAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task DeleteAsync(Guid reviewId,CancellationToken cancellationToken)
    {
        var review = await GetReviewByIdAsync(reviewId, cancellationToken);
        if (review != null)
        {
            _dbContext.Reviews.Remove(review);
        }
    }
    public async Task DeleteCommentAsync(Guid commentId,CancellationToken cancellationToken)
    {
        var comment = await GetCommentByIdAsync(commentId, cancellationToken);
        if(comment!=null)
        {
            _dbContext.Comments.Remove(comment);
        }
    }
    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
