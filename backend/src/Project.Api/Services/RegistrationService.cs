using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        // Guard: allowed roles only
        if (request.Role != RoleNames.Student && request.Role != RoleNames.Teacher)
        {
            return RegistrationResult.Invalid(new Dictionary<string, string[]>
            {
                ["role"] = new[] { "NotAllowed" }
            });
        }

        // Existing email or username
        var existing = await FindConflictAsync(request);
        if (existing != null)
        {
            return existing;
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
        };

        IdentityResult createResult;
        try
        {
            createResult = await _userManager.CreateAsync(user, request.Password);
        }
        catch (DbUpdateException)
        {
            // A concurrent request may have saved the same user between the checks and the save.
            var conflict = await FindConflictAsync(request);
            if (conflict is null) throw;
            _logger.LogWarning("Registration conflicted with a concurrent request");
            return conflict;
        }
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

    private async Task<RegistrationResult?> FindConflictAsync(RegisterRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
        {
            return RegistrationResult.Conflict(new Dictionary<string, string[]> { ["email"] = new[] { "AlreadyRegistered" } });
        }

        if (await _userManager.FindByNameAsync(request.UserName) != null)
        {
            return RegistrationResult.Conflict(new Dictionary<string, string[]> { ["userName"] = new[] { "AlreadyInUse" } });
        }

        return null;
    }
}
