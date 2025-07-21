using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zaczytani.Domain.Entities;

public class Review
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(300)]
    public string? Content { get; set; }

    [Range(1, 10)]
    public int? Rating { get; set; }

    [Required]
    public int Progress { get; set; }

    public List<Guid> Likes { get; set; } = new();

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public bool IsFinal { get; set; } = false;

    public bool ContainsSpoilers { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Book))]
    public Guid BookId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual List<Comment> Comments { get; set; } = [];
}
