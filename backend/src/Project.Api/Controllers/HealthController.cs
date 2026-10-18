using Microsoft.AspNetCore.Mvc;
using Project.Api.DTOs.Responses;

namespace Project.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new HealthResponse { Status = "ok" });
}
