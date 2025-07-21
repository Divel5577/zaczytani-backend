using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zaczytani.Domain.Enums;

namespace Zaczytani.Domain.Entities;

public class Book
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = null!;

    [MinLength(10)]
    [MaxLength(13)]
    public string Isbn { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = null!;

    [Required]
    public int PageNumber { get; set; }

    [Required]
    public DateOnly ReleaseDate { get; set; }

    public string? Image { get; set; }

    public List<BookGenre> Genre { get; set; } = [];

    public string? Series { get; set; }

    /// <summary>
    /// Admin who created the book.
    /// Need for future support and debugging.
    /// </summary>
    [ForeignKey(nameof(User))]
    public Guid? UserId { get; set; }

    public virtual User? User { get; set; }

    public virtual PublishingHouse PublishingHouse { get; set; } = null!;

    public virtual List<Author> Authors { get; set; } = [];

    public virtual List<Review> Reviews { get; set; } = [];
}
