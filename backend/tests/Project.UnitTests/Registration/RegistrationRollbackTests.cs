using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Registration;

public class RegistrationRollbackTests
{
    [Fact]
    public async Task RegisterAsync_WhenRoleAssignmentFails_DeletesUserAndReturnsRoleError()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var userManager = CreateFailingUserManager(scope, deleteSucceeds: true);
        var service = CreateService(scope, userManager);

        var result = await service.RegisterAsync(CreateRequest());

        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.Contains("NotAllowed", result.Errors["role"]);
        Assert.Null(await userManager.FindByNameAsync("rollback_user"));
        Assert.DoesNotContain(factory.Logs, m => m.Contains("rollback failed"));
    }

    [Fact]
    public async Task RegisterAsync_WhenRoleAssignmentAndRollbackFail_LogsErrorAndReturnsRoleError()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var userManager = CreateFailingUserManager(scope, deleteSucceeds: false);
        var service = CreateService(scope, userManager);

        var result = await service.RegisterAsync(CreateRequest());

        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.Contains("NotAllowed", result.Errors["role"]);
        var storedUser = await userManager.FindByNameAsync("rollback_user");
        Assert.NotNull(storedUser);
        Assert.Contains(factory.Logs, m => m.Contains("rollback failed") && m.Contains(storedUser.Id));
    }

    private static RegisterRequest CreateRequest() => new()
    {
        Email = "rollback@example.com",
        UserName = "rollback_user",
        Password = "StrongPassword123",
        Role = RoleName.Student
    };

    private static FailingUserManager CreateFailingUserManager(IServiceScope scope, bool deleteSucceeds)
    {
        var userManager = ActivatorUtilities.CreateInstance<FailingUserManager>(scope.ServiceProvider);
        userManager.DeleteSucceeds = deleteSucceeds;
        return userManager;
    }

    private static RegistrationService CreateService(IServiceScope scope, UserManager<ApplicationUser> userManager)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RegistrationService>>();
        return new RegistrationService(userManager, logger);
    }

    private class FailingUserManager : UserManager<ApplicationUser>
    {
        public FailingUserManager(
            IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public bool DeleteSucceeds { get; set; }

        public override Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role) =>
            Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "RoleFailed" }));

        public override Task<IdentityResult> DeleteAsync(ApplicationUser user) =>
            DeleteSucceeds ? base.DeleteAsync(user) : Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "DeleteFailed" }));
    }
}
