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
    private readonly ITeacherTaskService _teacherTaskService;

    public ClassTaskController(
        IGbifTaskService gbifTaskService,
        IClassTaskService classTaskService,
        ITeacherTaskService teacherTaskService)
    {
        _gbifTaskService = gbifTaskService;
        _classTaskService = classTaskService;
        _teacherTaskService = teacherTaskService;
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

    [HttpPost("tasks")]
    [ProducesResponseType(typeof(ClassTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTeacherTask(Guid id, [FromBody] CreateTeacherTaskRequest request)
    {
        var result = await _teacherTaskService.CreateAsync(User.GetRequiredUserId(), id, request, HttpContext.RequestAborted);

        return result.Outcome switch
        {
            TeacherTaskOutcome.Created => Created($"/api/classes/{id}/tasks", result.Task),
            TeacherTaskOutcome.Forbidden => Forbid(),
            _ => NotFound()
        };
    }

    [HttpDelete("tasks/{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteTeacherTask(Guid id, Guid taskId)
    {
        var result = await _teacherTaskService.DeleteAsync(User.GetRequiredUserId(), id, taskId, HttpContext.RequestAborted);

        return result.Outcome switch
        {
            TeacherTaskOutcome.Deleted => NoContent(),
            TeacherTaskOutcome.Forbidden => Forbid(),
            TeacherTaskOutcome.NotTeacherTask => Conflict(),
            _ => NotFound()
        };
    }
}
