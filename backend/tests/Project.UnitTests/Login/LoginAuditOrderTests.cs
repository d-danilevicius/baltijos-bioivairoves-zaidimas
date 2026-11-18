using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Login;

public class LoginAuditOrderTests
{
    private const string Email = "audit@example.com";
    private const string Password = "CorrectPassword1!";

    [Fact]
    public async Task LoginAsync_WithValidCredentials_WritesAuditBeforeSignIn()
    {
        var events = new List<string>();
        var loginService = await CreateServiceAsync(events, auditFails: false);

        var result = await loginService.LoginAsync(CreateRequest(), CancellationToken.None);

        Assert.Equal(LoginOutcome.Succeeded, result.Outcome);
        Assert.Equal(new[] { ActionName.Login.Success, "SignIn" }, events);
    }

    [Fact]
    public async Task LoginAsync_WhenAuditWriteFails_DoesNotSignIn()
    {
        var events = new List<string>();
        var loginService = await CreateServiceAsync(events, auditFails: true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => loginService.LoginAsync(CreateRequest(), CancellationToken.None));

        Assert.DoesNotContain("SignIn", events);
    }

    private static LoginRequest CreateRequest() => new() { Email = Email, Password = Password };

    private static async Task<LoginService> CreateServiceAsync(List<string> events, bool auditFails)
    {
        var factory = await Registration.TestServiceFactory.CreateAsync();
        var scope = factory.ServiceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser { UserName = "audit_user", Email = Email };
        await userManager.CreateAsync(user, Password);
        await userManager.AddToRoleAsync(user, RoleName.Student);

        var signInManager = ActivatorUtilities.CreateInstance<RecordingSignInManager>(scope.ServiceProvider);
        signInManager.Events = events;
        var writer = new RecordingActionLogWriter(events, auditFails);
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LoginService>>();
        return new LoginService(userManager, signInManager, TimeProvider.System, logger, writer);
    }

    private sealed class RecordingActionLogWriter : IActionLogWriter
    {
        private readonly List<string> _events;
        private readonly bool _fails;

        public RecordingActionLogWriter(List<string> events, bool fails)
        {
            _events = events;
            _fails = fails;
        }

        public Task WriteAsync(string? userId, string action, DateTime occurredAtUtc, CancellationToken cancellationToken)
        {
            if (_fails) throw new InvalidOperationException("Audit write failed");
            _events.Add(action);
            return Task.CompletedTask;
        }
    }

    private sealed class RecordingSignInManager : SignInManager<ApplicationUser>
    {
        public RecordingSignInManager(
            UserManager<ApplicationUser> userManager,
            Microsoft.AspNetCore.Http.IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            Microsoft.Extensions.Options.IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public List<string> Events { get; set; } = new();

        public override Task SignInAsync(ApplicationUser user, bool isPersistent, string? authenticationMethod = null)
        {
            Events.Add("SignIn");
            return Task.CompletedTask;
        }
    }
}
