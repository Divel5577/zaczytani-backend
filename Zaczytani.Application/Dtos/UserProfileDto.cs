using Zaczytani.Domain.Entities;

namespace Zaczytani.Application.Dtos;

public class UserProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int TotalBooksRead { get; set; }
    public List<string> FavoriteGenres { get; set; } = new();
    public List<BadgeDto>? Badges { get; set; }
    public IEnumerable<BookDto> ReadBooks { get; set; } = [];
    public IEnumerable<BookDto> CurrentlyReading { get; set; } = [];
    public IEnumerable<ChallengeProgressDto> ChallengeProgresses { get; set; } = [];
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsFollowed { get; set; }

}
