using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;
using Project.Api.Entities;

namespace Project.Api.Services;

public class LoginService : ILoginService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly System.TimeProvider _timeProvider;
    private readonly ILogger<LoginService> _logger;
    private readonly IActionLogWriter _actionLogWriter;

    public LoginService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        System.TimeProvider timeProvider,
        ILogger<LoginService> logger,
        IActionLogWriter actionLogWriter)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _timeProvider = timeProvider;
        _logger = logger;
        _actionLogWriter = actionLogWriter;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var email = request.Email ?? string.Empty;
        var password = request.Password ?? string.Empty;

        var user = await _userManager.FindByEmailAsync(email);

        var occurred = _timeProvider.GetUtcNow().UtcDateTime;

        if (user == null)
        {
            await _actionLogWriter.WriteAsync(null, ActionNames.Login.Failed, occurred, cancellationToken);
            _logger.LogInformation("Login failed for unknown account");
            return LoginResult.InvalidCredentials();
        }

        var check = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!check.Succeeded)
        {
            await _actionLogWriter.WriteAsync(user.Id, ActionNames.Login.Failed, occurred, cancellationToken);
            _logger.LogInformation("Login failed for user {UserId}", user.Id);
            return LoginResult.InvalidCredentials();
        }

        // password correct
        if (user.IsBlocked)
        {
            await _actionLogWriter.WriteAsync(user.Id, ActionNames.Login.Failed, occurred, cancellationToken);
            _logger.LogInformation("Login attempt for blocked user {UserId}", user.Id);
            return LoginResult.Blocked();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        await _actionLogWriter.WriteAsync(user.Id, ActionNames.Login.Success, occurred, cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        var resp = new LoggedInUserResponse
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Role = role
        };

        _logger.LogInformation("Login succeeded for user {UserId}", user.Id);
        return LoginResult.Succeeded(resp);
    }

    public async Task LogoutAsync(string userId, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
        var occurred = _timeProvider.GetUtcNow().UtcDateTime;
        await _actionLogWriter.WriteAsync(userId, ActionNames.Logout, occurred, cancellationToken);
        _logger.LogInformation("Logout for user {UserId}", userId ?? "<null>");
    }
}
