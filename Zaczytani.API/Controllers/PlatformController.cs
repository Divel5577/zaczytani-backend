using Microsoft.AspNetCore.Mvc;

namespace Zaczytani.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformController : ControllerBase
{
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok(DateTime.UtcNow);
    }
}

