using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zaczytani.Domain.Entities;

public class UserDrawnBook
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Book))]
    public Guid BookId { get; set; }
    public virtual Book Book { get; set; } = null!;

    public DateTime DrawnDate { get; set; }
    public bool IsRead { get; set; }
}
