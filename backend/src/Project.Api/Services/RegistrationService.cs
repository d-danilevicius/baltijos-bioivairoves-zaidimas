using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;
using Project.Api.Entities;
using System.Collections.Generic;

namespace Project.Api.Services;

public class RegistrationService : IRegistrationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(UserManager<ApplicationUser> userManager, ILogger<RegistrationService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<RegistrationResult> RegisterAsync(RegisterRequest request)
    {
        // Guard: allowed roles only
        if (request.Role != RoleNames.Student && request.Role != RoleNames.Teacher)
        {
            return RegistrationResult.Invalid(new Dictionary<string, string[]>
            {
                ["role"] = new[] { "NotAllowed" }
            });
        }

        // Existing email
        var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingByEmail != null)
        {
            return RegistrationResult.Conflict(new Dictionary<string, string[]>
            {
                ["email"] = new[] { "AlreadyRegistered" }
            });
        }

        // Existing username
        var existingByName = await _userManager.FindByNameAsync(request.UserName);
        if (existingByName != null)
        {
            return RegistrationResult.Conflict(new Dictionary<string, string[]>
            {
                ["userName"] = new[] { "AlreadyInUse" }
            });
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            // Map identity errors to contract errors or throw on unknown
            return IdentityErrorMapper.Map(createResult.Errors, _logger);
        }

        // Assign role
        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
        {
            // rollback: delete the created user
            await _userManager.DeleteAsync(user);

            var errors = new Dictionary<string, string[]> { ["role"] = new[] { "NotAllowed" } };
            return RegistrationResult.Invalid(errors);
        }

        var created = new RegisteredUserResponse
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Role = request.Role
        };

        return RegistrationResult.Created(created);
    }
}
