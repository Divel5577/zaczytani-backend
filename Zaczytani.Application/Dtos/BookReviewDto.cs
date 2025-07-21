namespace Zaczytani.Application.Dtos;

public class BookReviewDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
    public int Likes { get; set; }
    public bool IsLiked { get; set; }
    public int Comments { get; set; }
    public int NotesCount { get; set; }
    public UserDto User { get; set; }
}
