using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Api.Authorization;
using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;
using Project.Api.Services;

namespace Project.Api.Controllers;

[ApiController]
[Route("api/my/tasks/{taskId:guid}/submissions")]
[Authorize(Policy = PolicyName.Student)]
public class StudentSubmissionController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public StudentSubmissionController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubmissionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(Guid taskId, [FromBody] CreateSubmissionRequest request)
    {
        var result = await _submissionService.CreateAsync(User.GetRequiredUserId(), taskId, request, HttpContext.RequestAborted);

        return result.Outcome switch
        {
            SubmissionOutcome.Created => Created($"/api/my/tasks/{taskId}", result.Submission),
            SubmissionOutcome.InvalidAnswer => InvalidAnswer(),
            SubmissionOutcome.Locked => Forbid(),
            SubmissionOutcome.AlreadySubmitted => Conflict(),
            _ => NotFound()
        };
    }

    private IActionResult InvalidAnswer()
    {
        ModelState.AddModelError("answer", "Invalid");
        return ValidationProblem(ModelState);
    }
}
