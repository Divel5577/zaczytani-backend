using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zaczytani.Domain.Entities;

public class Comment
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(300)]
    public string Content { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    [Required]
    [ForeignKey(nameof(Review))]
    public Guid ReviewId { get; set; }
    public virtual Review Review { get; set; }
}
