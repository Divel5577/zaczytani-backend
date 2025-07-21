using Zaczytani.Domain.Enums;

namespace Zaczytani.Application.Dtos;

public class ReportDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public ReportCategory Category { get; set; }
    public UserDto User { get; set; }
    public string Review { get; set; }

}
