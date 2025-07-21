namespace Zaczytani.Infrastructure.Configuration;

public class FileStorageOptions
{
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string Windows { get; set; } = string.Empty;
    public string Linux { get; set; } = string.Empty;
    public string DefaultFolder { get; set; } = string.Empty;
}
