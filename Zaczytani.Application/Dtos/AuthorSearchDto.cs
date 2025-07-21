using Zaczytani.Domain.Enums;

namespace Zaczytani.Application.Dtos;

public class AuthorSearchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public int BookCount { get; set; } 
    public IEnumerable<BookGenre> Genres { get; set; }
}
