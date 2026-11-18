using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Api.Authorization;

namespace Project.IntegrationTests.AccessControl;

[ApiController]
[Route("api/test-policies")]
public class PolicyTestController : ControllerBase
{
    [HttpGet("student")]
    [Authorize(Policy = PolicyName.Student)]
    public IActionResult Student()
    {
        return Ok();
    }

    [HttpGet("teacher")]
    [Authorize(Policy = PolicyName.Teacher)]
    public IActionResult Teacher()
    {
        return Ok();
    }

    [HttpGet("administrator")]
    [Authorize(Policy = PolicyName.Administrator)]
    public IActionResult Administrator()
    {
        return Ok();
    }
}
