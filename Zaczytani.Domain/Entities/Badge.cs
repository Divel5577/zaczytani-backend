using System.ComponentModel.DataAnnotations;
using Zaczytani.Domain.Enums;

namespace Zaczytani.Domain.Entities;

public class Badge
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public BadgeType Type { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string IconPath { get; set; }

    public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}
