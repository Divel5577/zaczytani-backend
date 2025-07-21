namespace Zaczytani.Application.Dtos;

public class ReviewDetailsDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
    public int Likes { get; set; }
    public bool IsLiked { get; set; }
    public UserDto User { get; set; }
    public ReviewDetailsBookDto Book { get; set; }
    public IEnumerable<NoteDto> Notes { get; set; }
    public IEnumerable<CommentDto> Comments { get; set; }
}

public class NoteDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
    public int Progress { get; set; }
    public bool ContainsSpoilers { get; set; }
}

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public UserDto User { get; set; }
}


public class ReviewDetailsBookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? ImageUrl { get; set; }
    public string? Series { get; set; }
    public int PageNumber { get; set; }
    public IEnumerable<AuthorDto> Authors { get; set; }
}