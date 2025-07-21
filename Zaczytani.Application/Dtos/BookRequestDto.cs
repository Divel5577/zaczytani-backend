using Zaczytani.Domain.Enums;

namespace Zaczytani.Application.Dtos;

public class BookRequestDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public string? Description { get; set; }
    public int? PageNumber { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public string? FileName { get; set; }
    public string Authors { get; set; } = string.Empty;
    public string? PublishingHouse { get; set; }
    public List<BookGenre> Genre { get; set; } = [];
    public string? Series { get; set; }
    public UserDto User { get; set; } = new();
}
