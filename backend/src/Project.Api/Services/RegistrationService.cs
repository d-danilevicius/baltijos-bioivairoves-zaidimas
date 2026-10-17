using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;
using Project.Api.Entities;

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
        // Validate allowed role
        if (request.Role != RoleNames.Student && request.Role != RoleNames.Teacher)
        {
            return new RegistrationResult
            {
                Success = false,
                Errors = new[] { "role:NotAllowed" }
            };
        }

        // Check existing email (ignore case) using UserManager which normalizes the email
        var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingByEmail != null)
        {
            return new RegistrationResult
            {
                Success = false,
                Errors = new[] { "email:AlreadyRegistered" }
            };
        }

        // Check existing username
        var existingByName = await _userManager.FindByNameAsync(request.Username);
        if (existingByName != null)
        {
            return new RegistrationResult
            {
                Success = false,
                Errors = new[] { "userName:AlreadyInUse" }
            };
        }

        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            // Map identity errors to contract error codes where possible
            var errors = createResult.Errors.Select(e => e.Code).ToArray();
            return new RegistrationResult
            {
                Success = false,
                Errors = errors
            };
        }

        // Assign role
        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
        {
            // rollback: delete the created user
            await _userManager.DeleteAsync(user);
            var errors = roleResult.Errors.Select(e => e.Code).ToArray();
            return new RegistrationResult
            {
                Success = false,
                Errors = errors
            };
        }

        var created = new RegisteredUserResponse
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Role = request.Role
        };

        return new RegistrationResult
        {
            Success = true,
            CreatedUser = created
        };
    }
}
