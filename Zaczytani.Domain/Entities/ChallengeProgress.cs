using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zaczytani.Domain.Entities;

public class ChallengeProgress
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Challenge))]
    public Guid ChallengeId { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;

    public int BooksRead { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.Now;

    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
