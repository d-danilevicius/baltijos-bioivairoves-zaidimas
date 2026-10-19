using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;
using Project.Api.Services;

namespace Project.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly ILoginService _loginService;
    private readonly IAntiforgery _antiforgery;

    public AuthController(IRegistrationService registrationService, ILoginService loginService, IAntiforgery antiforgery)
    {
        _registrationService = registrationService;
        _loginService = loginService;
        _antiforgery = antiforgery;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [ProducesResponseType(typeof(RegisteredUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _registrationService.RegisterAsync(request);

        if (result.Success)
        {
            // A successful registration must include the created user data
            if (result.CreatedUser == null)
            {
                throw new InvalidOperationException("RegistrationService returned Success=true but CreatedUser is null");
            }

            // 201 Created with the created user response
            return Created(string.Empty, result.CreatedUser);
        }

        if (result.Errors == null)
        {
            // No structured errors - treat as bad request
            return BadRequest(new ValidationProblemDetails());
        }

        var details = new ValidationProblemDetails(result.Errors.ToDictionary(k => k.Key, v => v.Value));
        if (result.IsConflict)
            return Conflict(details);

        return BadRequest(details);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [ProducesResponseType(typeof(LoggedInUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _loginService.LoginAsync(request, HttpContext.RequestAborted);
        return MapLoginResult(result);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await _loginService.LogoutAsync(userId, HttpContext.RequestAborted);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
        {
            return Unauthorized();
        }

        var response = new CurrentUserResponse
        {
            Id = id,
            UserName = userName,
            Email = email,
            Role = role
        };

        return Ok(response);
    }

    [HttpGet("csrf")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [ProducesResponseType(typeof(CsrfTokenResponse), StatusCodes.Status200OK)]
    public IActionResult Csrf()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new CsrfTokenResponse { Token = tokens.RequestToken! });
    }

    private IActionResult MapLoginResult(LoginResult result)
    {
        if (result.Outcome == LoginOutcome.Succeeded)
        {
            if (result.User == null)
            {
                throw new InvalidOperationException("LoginService returned Succeeded but User is null");
            }

            return Ok(result.User);
        }

        if (result.Outcome == LoginOutcome.InvalidCredentials)
        {
            var details = new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [""] = new[] { "InvalidCredentials" }
            })
            {
                Status = StatusCodes.Status401Unauthorized,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Unauthorized",
                Detail = "Invalid credentials."
            };

            return new UnauthorizedObjectResult(details);
        }

        if (result.Outcome == LoginOutcome.Blocked)
        {
            var details = new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [""] = new[] { "AccountBlocked" }
            })
            {
                Status = StatusCodes.Status403Forbidden,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4",
                Title = "Forbidden",
                Detail = "Account is blocked."
            };

            return new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        throw new InvalidOperationException($"Unexpected login outcome: {result.Outcome}");
    }
}
