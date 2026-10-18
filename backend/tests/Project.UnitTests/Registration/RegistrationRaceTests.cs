using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Registration
{
    // Simulates a concurrent request that saves the same user first, so that the database
    // rejects this request with a unique constraint violation while the user is created.
    internal class RacingUserManager : UserManager<ApplicationUser>
    {
        public RacingUserManager(IServiceProvider services)
            : base(
                services.GetRequiredService<IUserStore<ApplicationUser>>(),
                services.GetRequiredService<IOptions<IdentityOptions>>(),
                services.GetRequiredService<IPasswordHasher<ApplicationUser>>(),
                services.GetServices<IUserValidator<ApplicationUser>>(),
                services.GetServices<IPasswordValidator<ApplicationUser>>(),
                services.GetRequiredService<ILookupNormalizer>(),
                services.GetRequiredService<IdentityErrorDescriber>(),
                services,
                services.GetRequiredService<ILogger<UserManager<ApplicationUser>>>())
        {
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            var winner = new ApplicationUser { UserName = user.UserName, Email = user.Email };
            await base.CreateAsync(winner, password);
            throw new DbUpdateException("Unique constraint violated.");
        }
    }

    public class RegistrationRaceTests
    {
        [Fact]
        public async Task RegisterAsync_WhenDatabaseRejectsDuplicateEmailDuringCreate_ReturnsConflict()
        {
            using var factory = await TestServiceFactory.CreateAsync();
            using var scope = factory.ServiceProvider.CreateScope();

            var userManager = new RacingUserManager(scope.ServiceProvider);
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<RegistrationService>>();
            var service = new RegistrationService(userManager, logger);

            var request = new RegisterRequest
            {
                Email = "race@example.com",
                UserName = "race_user",
                Password = "StrongPassword123",
                Role = RoleNames.Student
            };

            var result = await service.RegisterAsync(request);

            Assert.False(result.Success);
            Assert.True(result.IsConflict);
            Assert.Contains("AlreadyRegistered", result.Errors!["email"]);
        }
    }
}
