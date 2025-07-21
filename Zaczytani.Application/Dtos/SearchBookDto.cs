namespace Zaczytani.Application.Dtos;

public record SearchBookDto(Guid Id, string Name, string? ImageUrl, IEnumerable<BookSearchDto> Books);
