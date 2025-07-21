using Microsoft.AspNetCore.Http;

namespace Zaczytani.Domain.Repositories;

public interface IFileStorageRepository
{
    Task<string> SaveFileAsync(IFormFile file);
    Task<byte[]?> GetFileAsync(string fileName);
    string? GetFilePath(string fileName);
    string? GetFileUrl(string? fileName);
}
