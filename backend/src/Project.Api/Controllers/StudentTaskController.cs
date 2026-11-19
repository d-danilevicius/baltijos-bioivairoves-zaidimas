using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Api.Authorization;
using Project.Api.DTOs.Responses;
using Project.Api.Services;

namespace Project.Api.Controllers;

[ApiController]
[Route("api/my/tasks")]
[Authorize(Policy = PolicyName.Student)]
public class StudentTaskController : ControllerBase
{
    private readonly IStudentTaskService _studentTaskService;
    private readonly IStudentTaskDetailsService _studentTaskDetailsService;

    public StudentTaskController(
        IStudentTaskService studentTaskService,
        IStudentTaskDetailsService studentTaskDetailsService)
    {
        _studentTaskService = studentTaskService;
        _studentTaskDetailsService = studentTaskDetailsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(StudentTaskListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyTasks()
    {
        var result = await _studentTaskService.GetAsync(User.GetRequiredUserId(), HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudentTaskDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyTask(Guid id)
    {
        var result = await _studentTaskDetailsService.GetAsync(User.GetRequiredUserId(), id, HttpContext.RequestAborted);

        return result.Outcome switch
        {
            StudentTaskDetailsOutcome.Found => Ok(result.Task),
            StudentTaskDetailsOutcome.Locked => Forbid(),
            _ => NotFound()
        };
    }
}
