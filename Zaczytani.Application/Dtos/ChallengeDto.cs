using Zaczytani.Domain.Enums;

namespace Zaczytani.Application.Dtos;

public class ChallengeDto
{
    public Guid Id { get; set; }
    public int BooksToRead { get; set; }
    public string? CriteriaValue { get; set; }
    public ChallengeType Criteria { get; set; }
}
