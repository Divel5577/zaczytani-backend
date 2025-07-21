using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zaczytani.Domain.Enums;

namespace Zaczytani.Domain.Entities;

public class Challenge
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    /// <summary>
    /// User who created the Challenge.
    /// </summary>
    public virtual User User { get; set; } = null!;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Required]
    public int BooksToRead { get; set; }

    [Required]
    public ChallengeType Criteria { get; set; }

    public string? CriteriaValue { get; set; } = null!;

    public List<ChallengeProgress> UserProgress { get; set; } = [];

}
