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

    public StudentTaskController(IStudentTaskService studentTaskService)
    {
        _studentTaskService = studentTaskService;
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
}
