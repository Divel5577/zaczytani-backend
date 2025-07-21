namespace Zaczytani.Application.Dtos;

public class ReadingBookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? ImageUrl { get; set; }
    public string? Series { get; set; }
    public int? Progress { get; set; }
    public int PageNumber { get; set; }
    public IEnumerable<AuthorDto> Authors { get; set; }
}
