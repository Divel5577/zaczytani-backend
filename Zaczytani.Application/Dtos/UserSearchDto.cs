using Zaczytani.Domain.Enums;

namespace Zaczytani.Application.Dtos;

public class UserSearchDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int ReadBookCount { get; set; }
    public IEnumerable<BookGenre> BookGenres { get; set; } = [];
}
