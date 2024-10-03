using Microsoft.AspNetCore.Mvc;

namespace LoginSystem.Api.Controllers.Internal;

[Route("internal/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)] // Hide from swagger
public class VersionController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult Get()
    {
        return Ok(typeof(Program).Assembly.GetName().Version?.ToString());
    }
}
