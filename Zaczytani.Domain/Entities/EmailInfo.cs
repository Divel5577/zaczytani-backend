using Zaczytani.Domain.Enums;

namespace Zaczytani.Domain.Entities;

public class EmailInfo
{
    public Guid Id { get; set; }
    public string EmailTo { get; set; } = string.Empty;
    public EmailTemplate EmailTemplate { get; set; }
    public string[] EmailContent { get; set; } = [];
    public int CurrentRetry { get; set; } = 0;
    public int MaxRetries { get; set; } = 10;
    public bool IsSent { get; set; }
}
