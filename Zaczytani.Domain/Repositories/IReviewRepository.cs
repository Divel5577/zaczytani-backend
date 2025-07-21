using Zaczytani.Domain.Entities;

namespace Zaczytani.Domain.Repositories;

public interface IReviewRepository
{
    Task AddAsync(Review entity, CancellationToken cancellationToken);
    Task AddCommentAsync(Comment entity, CancellationToken cancellationToken);
    Task<IEnumerable<Review>> GetFinalReviewsByBookId(Guid bookId, CancellationToken cancellationToken);
    Task<Review?> GetLatestReviewByBookIdAsync(Guid bookId, Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<Review>> GetReviewsByBookIdAndUserId(Guid bookId, Guid userId, CancellationToken cancellationToken);
    Task<Review?> GetReviewByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Review?> GetFinalReviewByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Comment?> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<int> CountUserReviewsAsync(Guid userId, CancellationToken cancellationToken);
    Task<int> CountUserCommentsAsync(Guid userId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid reviewId, CancellationToken cancellationToken);
    Task DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}