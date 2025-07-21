using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Zaczytani.Domain.Enums;

namespace Zaczytani.Domain.Entities;

public class Report
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Content { get; set; } = null!;

    public ReportCategory Category { get; set; }

    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    /// <summary>
    /// User who created report
    /// </summary>
    public virtual User User { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Review))]
    public Guid ReviewId { get; set; }

    public virtual Review Review { get; set; } = null!;
}
