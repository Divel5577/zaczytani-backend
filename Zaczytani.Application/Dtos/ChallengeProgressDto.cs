using Zaczytani.Domain.Enums;

namespace Zaczytani.Application.Dtos;

public class ChallengeProgressDto
{
    public Guid Id { get; set; }
    public Guid ChallengeId { get; set; }
    public int BooksToRead { get; set; }
    public string? CriteriaValue { get; set; }
    public ChallengeType Criteria { get; set; }
    public int BooksRead { get; set; }
    public bool IsCompleted { get; set; }
}
