using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Api.Authorization;
using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;
using Project.Api.Services;

namespace Project.Api.Controllers;

[ApiController]
[Route("api/classes/{id:guid}")]
[Authorize(Policy = PolicyName.Teacher)]
public class ClassTaskController : ControllerBase
{
    private readonly IGbifTaskService _gbifTaskService;
    private readonly IClassTaskService _classTaskService;

    public ClassTaskController(IGbifTaskService gbifTaskService, IClassTaskService classTaskService)
    {
        _gbifTaskService = gbifTaskService;
        _classTaskService = classTaskService;
    }

    [HttpPost("gbif-tasks")]
    [ProducesResponseType(typeof(GbifTasksResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> CreateGbifTasks(Guid id, [FromBody] CreateGbifTasksRequest request)
    {
        var result = await _gbifTaskService.CreateAsync(
            User.GetRequiredUserId(),
            id,
            request.Latitude.GetValueOrDefault(),
            request.Longitude.GetValueOrDefault(),
            request.Season.GetValueOrDefault(),
            HttpContext.RequestAborted);

        return result.Outcome switch
        {
            GbifTaskOutcome.Created => Created($"/api/classes/{id}/tasks", new GbifTasksResponse { CreatedCount = result.CreatedCount }),
            GbifTaskOutcome.Forbidden => Forbid(),
            GbifTaskOutcome.AlreadyCreated => Conflict(),
            GbifTaskOutcome.TooFewObservations => StatusCode(StatusCodes.Status422UnprocessableEntity),
            GbifTaskOutcome.GbifUnavailable => StatusCode(StatusCodes.Status502BadGateway),
            _ => NotFound()
        };
    }

    [HttpGet("tasks")]
    [ProducesResponseType(typeof(IReadOnlyList<ClassTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListTasks(Guid id)
    {
        var result = await _classTaskService.ListAsync(User.GetRequiredUserId(), id, HttpContext.RequestAborted);

        return result.Outcome switch
        {
            ClassOutcome.Succeeded => Ok(result.Tasks),
            ClassOutcome.Forbidden => Forbid(),
            _ => NotFound()
        };
    }
}
