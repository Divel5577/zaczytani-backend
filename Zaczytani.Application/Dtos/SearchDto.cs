namespace Zaczytani.Application.Dtos;

public record SearchDto(IEnumerable<BookSearchAuthorsDto> Books, IEnumerable<AuthorSearchDto> Authors, IEnumerable<UserSearchDto> Users);
