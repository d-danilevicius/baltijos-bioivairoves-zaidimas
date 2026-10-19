using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Api.Authorization;
using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;
using Project.Api.Services;

namespace Project.Api.Controllers;

[ApiController]
[Route("api/classes")]
[Authorize(Policy = PolicyNames.Teacher)]
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;

    public ClassesController(IClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClassResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List()
    {
        var classes = await _classService.ListAsync(GetOwnerId(), HttpContext.RequestAborted);
        return Ok(classes);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClassResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateClassRequest request)
    {
        var created = await _classService.CreateAsync(GetOwnerId(), request, HttpContext.RequestAborted);
        return Created($"/api/classes/{created.Id}", created);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClassResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _classService.GetAsync(GetOwnerId(), id, HttpContext.RequestAborted);
        return MapResult(result, () => Ok(result.Class));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _classService.DeleteAsync(GetOwnerId(), id, HttpContext.RequestAborted);
        return MapResult(result, NoContent);
    }

    private string GetOwnerId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private IActionResult MapResult(ClassResult result, Func<IActionResult> onSucceeded)
    {
        return result.Outcome switch
        {
            ClassOutcome.Succeeded => onSucceeded(),
            ClassOutcome.Forbidden => Forbid(),
            _ => NotFound()
        };
    }
}
