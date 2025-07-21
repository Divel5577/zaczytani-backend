using Zaczytani.Application.Filters;

namespace Zaczytani.Application.Services;

public interface IBadgeAssignerService : IUserIdAssignable
{
    Task CheckBookBadgesAsync(int readCount, CancellationToken cancellationToken);
    Task CheckReviewBadgesAsync(int reviewCount, CancellationToken cancellationToken);
    Task CheckCommentBadgesAsync(int commentCount, CancellationToken cancellationToken);
    Task CheckAccountAgeBadgesAsync(CancellationToken cancellationToken);
}