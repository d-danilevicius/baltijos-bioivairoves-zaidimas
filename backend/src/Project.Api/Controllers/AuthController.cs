using System.Collections.Generic;
using System.Linq;
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
    private readonly IAntiforgery _antiforgery;

    public AuthController(IRegistrationService registrationService, IAntiforgery antiforgery)
    {
        _registrationService = registrationService;
        _antiforgery = antiforgery;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
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

    [HttpGet("csrf")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public IActionResult Csrf()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { token = tokens.RequestToken });
    }
}
