using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Project.Api.Data;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Login;

public class LoginServiceTests
{
    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsInvalidCredentials()
    {
        using var factory = await Registration.TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var user = new ApplicationUser { UserName = "user1", Email = "user1@example.com" };
        await userManager.CreateAsync(user, "CorrectPassword1!");
        await userManager.AddToRoleAsync(user, RoleNames.Student);

        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();
        var request = new LoginRequest { Email = "user1@example.com", Password = "WrongPassword" };

        var result = await loginService.LoginAsync(request, CancellationToken.None);

        Assert.Equal(LoginOutcome.InvalidCredentials, result.Outcome);
    }

    [Fact]
    public async Task LoginAsync_WithUnknownEmail_ReturnsInvalidCredentials()
    {
        using var factory = await Registration.TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();

        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();
        var request = new LoginRequest { Email = "unknown@example.com", Password = "Whatever1!" };

        var result = await loginService.LoginAsync(request, CancellationToken.None);

        Assert.Equal(LoginOutcome.InvalidCredentials, result.Outcome);
    }

    [Fact]
    public async Task LoginAsync_WithBlockedUserAndWrongPassword_ReturnsInvalidCredentials()
    {
        using var factory = await Registration.TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser { UserName = "blocked", Email = "blocked@example.com", IsBlocked = true };
        await userManager.CreateAsync(user, "CorrectPassword1!");
        await userManager.AddToRoleAsync(user, RoleNames.Student);

        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();
        var request = new LoginRequest { Email = "blocked@example.com", Password = "WrongPassword" };

        var result = await loginService.LoginAsync(request, CancellationToken.None);

        // Wrong password must not reveal the blocked status
        Assert.Equal(LoginOutcome.InvalidCredentials, result.Outcome);
    }

    [Fact]
    public async Task LoginAsync_WithBlockedUserAndCorrectPassword_ReturnsBlocked()
    {
        using var factory = await Registration.TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser { UserName = "blocked2", Email = "blocked2@example.com", IsBlocked = true };
        await userManager.CreateAsync(user, "CorrectPassword1!");
        await userManager.AddToRoleAsync(user, RoleNames.Student);

        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();
        var request = new LoginRequest { Email = "blocked2@example.com", Password = "CorrectPassword1!" };

        var result = await loginService.LoginAsync(request, CancellationToken.None);

        Assert.Equal(LoginOutcome.Blocked, result.Outcome);
    }

    [Fact]
    public async Task SessionCookieOptions_AreSlidingWithThirtyMinuteExpiration()
    {
        using var factory = await Registration.TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();

        var optionsMonitor = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
        var options = optionsMonitor.Get(IdentityConstants.ApplicationScheme);

        Assert.Equal(TimeSpan.FromMinutes(30), options.ExpireTimeSpan);
        Assert.True(options.SlidingExpiration);
    }
}
