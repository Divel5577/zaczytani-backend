using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController(IFileStorageRepository fileStorageRepository) : ControllerBase
{
    private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFile([FromRoute] string fileName)
    {
        var fileBytes = await _fileStorageRepository.GetFileAsync(fileName);

        return fileBytes is null ? NotFound() : File(fileBytes, "image/jpeg");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file was uploaded or the file is empty.");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest($"Unsupported file format. Allowed formats are: {string.Join(", ", allowedExtensions)}");
        }

        var fileName = await _fileStorageRepository.SaveFileAsync(file);

        return Ok(new { FileName = fileName });
    }
}
