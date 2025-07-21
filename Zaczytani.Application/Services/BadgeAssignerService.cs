using Microsoft.AspNetCore.Identity;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Services;

public class BadgeAssignerService(
    IBadgeRepository badgeRepository,
    UserManager<User> userManager
) : IBadgeAssignerService, IUserIdAssignable
{
    private readonly IBadgeRepository _badgeRepository = badgeRepository;
    private readonly UserManager<User> _userManager = userManager;
    private Guid _userId;

    public void SetUserId(Guid userId) => _userId = userId;

    public async Task CheckBookBadgesAsync(int readCount, CancellationToken cancellationToken)
    {
        await TryAssign(BadgeType.FirstBook, readCount >= 1, cancellationToken);
        await TryAssign(BadgeType.TenBooks, readCount >= 10, cancellationToken);
        await TryAssign(BadgeType.FiftyBooks, readCount >= 50, cancellationToken);
        await TryAssign(BadgeType.HundredBooks, readCount >= 100, cancellationToken);
        await TryAssign(BadgeType.TwoHundredBooks, readCount >= 200, cancellationToken);
    }

    public async Task CheckReviewBadgesAsync(int reviewCount, CancellationToken cancellationToken)
    {
        await TryAssign(BadgeType.FirstReview, reviewCount >= 1, cancellationToken);
        await TryAssign(BadgeType.Reviewer, reviewCount >= 5, cancellationToken);
        await TryAssign(BadgeType.LiteraryCritic, reviewCount >= 10, cancellationToken);
        await TryAssign(BadgeType.ReviewExpert, reviewCount >= 25, cancellationToken);
        await TryAssign(BadgeType.ReviewMaster, reviewCount >= 50, cancellationToken);
    }

    public async Task CheckCommentBadgesAsync(int commentCount, CancellationToken cancellationToken)
    {
        await TryAssign(BadgeType.FirstComment, commentCount >= 1, cancellationToken);
        await TryAssign(BadgeType.Commentator, commentCount >= 5, cancellationToken);
        await TryAssign(BadgeType.ActiveCommentator, commentCount >= 10, cancellationToken);
        await TryAssign(BadgeType.Debater, commentCount >= 25, cancellationToken);
        await TryAssign(BadgeType.DiscussionMaster, commentCount >= 50, cancellationToken);
    }

    public async Task CheckAccountAgeBadgesAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(_userId.ToString());
        if (user == null) return;

        var age = DateTime.UtcNow - user.CreateDate;

        await TryAssign(BadgeType.Newbie, age.TotalDays <= 90, cancellationToken);
        await TryAssign(BadgeType.RegularReader, age.TotalDays > 90 && age.TotalDays <= 365, cancellationToken);
        await TryAssign(BadgeType.Veteran, age.TotalDays > 365 && age.TotalDays <= 1095, cancellationToken);
        await TryAssign(BadgeType.LegendaryReader, age.TotalDays > 1095, cancellationToken);
    }

    private static string GetBadgeGroup(BadgeType type)
    {
        if (type.ToString().Contains("Review") || type.ToString().Contains("Critic")) return "Review";
        if (type.ToString().Contains("Book")) return "Book";
        if (type.ToString().Contains("Comment") ||  type.ToString().Contains("Debater") || type.ToString().Contains("Discussion")) return "Comment";
        if (type.ToString().Contains("Reader") || type.ToString().Contains("Newbie") || type.ToString().Contains("Veteran")) return "Account";
        return "Other";
    }

    private async Task TryAssign(BadgeType type, bool condition, CancellationToken cancellationToken)
    {
        if (!condition) return;

        var group = GetBadgeGroup(type);
        var userBadges = await _badgeRepository.GetUserBadgesAsync(_userId, cancellationToken);
        var groupBadges = userBadges
            .Where(ub => GetBadgeGroup(ub.Badge.Type) == group)
            .ToList();

        foreach (var badge in groupBadges)
        {
            await _badgeRepository.DeleteUserBadgeAsync(badge, cancellationToken);
        }

        var badgeDefinition = await _badgeRepository.GetBadgeDefinitionByTypeAsync(type, cancellationToken);
        if (badgeDefinition is null) return;

        await _badgeRepository.AddUserBadgeAsync(new UserBadge
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            BadgeId = badgeDefinition.Id
        }, cancellationToken);
    }
}
