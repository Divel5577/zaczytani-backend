using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Configuration;

namespace Zaczytani.Infrastructure.Repositories;

internal class FileStorageRepository : IFileStorageRepository
{
    private readonly string _baseFolder;
    private readonly string _apiBaseUrl;

    public FileStorageRepository(IOptions<FileStorageOptions> fileStorageOptions)
    {
        var osType = Environment.OSVersion.Platform;

        _baseFolder = osType switch
        {
            PlatformID.Win32NT => fileStorageOptions.Value.Windows,
            PlatformID.Unix => fileStorageOptions.Value.Linux,
            _ => fileStorageOptions.Value.DefaultFolder
        };
        _apiBaseUrl = fileStorageOptions.Value.ApiBaseUrl;
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        Directory.CreateDirectory(_baseFolder);

        var safeFileName = string.Concat(file.FileName.Split(Path.GetInvalidFileNameChars()))
                          .Replace(" ", "_");

        var fileName = $"{Guid.NewGuid()}_{safeFileName}";
        var filePath = Path.Combine(_baseFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }

    public async Task<byte[]?> GetFileAsync(string fileName)
    {
        var filePath = GetFilePath(fileName);
        if (filePath is null) return null;

        return await File.ReadAllBytesAsync(filePath);
    }

    public string? GetFilePath(string fileName)
    {
        var filePath = Path.Combine(_baseFolder, fileName);

        if (File.Exists(filePath))
        {
            return filePath;
        }

        return null;
    }

    public string? GetFileUrl(string? fileName)
    {
        if (fileName.IsNullOrEmpty())
        {
            return null;
        }

        return $"{_apiBaseUrl}{fileName}";
    }
}
