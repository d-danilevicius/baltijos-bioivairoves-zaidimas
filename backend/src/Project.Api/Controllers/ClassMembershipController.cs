using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Api.Authorization;
using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;
using Project.Api.Services;

namespace Project.Api.Controllers;

[ApiController]
[Route("api/classes/join")]
[Authorize(Policy = PolicyName.Student)]
public class ClassMembershipController : ControllerBase
{
    private readonly IClassMembershipService _membershipService;

    public ClassMembershipController(IClassMembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(JoinClassResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ClassChangeConfirmationResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Join([FromBody] JoinClassRequest request)
    {
        var result = await _membershipService.JoinAsync(User.GetRequiredUserId(), request, HttpContext.RequestAborted);

        return result.Outcome switch
        {
            JoinClassOutcome.Joined => Ok(new JoinClassResponse { ClassName = result.ClassName, AlreadyMember = false }),
            JoinClassOutcome.AlreadyMember => Ok(new JoinClassResponse { ClassName = result.ClassName, AlreadyMember = true }),
            JoinClassOutcome.ConfirmationRequired => Conflict(new ClassChangeConfirmationResponse
            {
                CurrentClassName = result.CurrentClassName,
                NewClassName = result.ClassName
            }),
            _ => ClassNotFound()
        };
    }

    private IActionResult ClassNotFound()
    {
        ModelState.AddModelError("code", "NotFound");
        return ValidationProblem(ModelState);
    }
}
