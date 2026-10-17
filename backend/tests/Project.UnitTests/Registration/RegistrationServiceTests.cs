using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Entities;
using Project.Api.DTOs.Requests;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Registration
{
    public class RegistrationServiceTests
    {
        [Fact]
        public async Task RegisterAsync_ValidStudent_ReturnsSuccess()
        {
            using var factory = await TestServiceFactory.CreateAsync();
            using var scope = factory.ServiceProvider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IRegistrationService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var request = new RegisterRequest
            {
                Email = "student@example.com",
                UserName = "student_user",
                Password = "StrongPassword123",
                Role = RoleNames.Student
            };

            var result = await service.RegisterAsync(request);

            Assert.True(result.Success);
            Assert.NotNull(result.CreatedUser);
            Assert.Equal(request.UserName, result.CreatedUser.UserName);
            Assert.Equal(RoleNames.Student, result.CreatedUser.Role);

            var stored = await userManager.FindByIdAsync(result.CreatedUser.Id);
            Assert.NotNull(stored);
            Assert.False(string.IsNullOrEmpty(stored.PasswordHash));
            Assert.NotEqual(request.Password, stored.PasswordHash);

            Assert.DoesNotContain(factory.Logs, m => m.Contains(request.Password));
        }

        [Fact]
        public async Task RegisterAsync_ValidTeacher_ReturnsSuccess()
        {
            using var factory = await TestServiceFactory.CreateAsync();
            using var scope = factory.ServiceProvider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IRegistrationService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var request = new RegisterRequest
            {
                Email = "teacher@example.com",
                UserName = "teacher_user",
                Password = "TeacherPass123",
                Role = RoleNames.Teacher
            };

            var result = await service.RegisterAsync(request);

            Assert.True(result.Success);
            Assert.NotNull(result.CreatedUser);
            Assert.Equal(request.UserName, result.CreatedUser.UserName);
            Assert.Equal(RoleNames.Teacher, result.CreatedUser.Role);

            var stored = await userManager.FindByIdAsync(result.CreatedUser.Id);
            Assert.NotNull(stored);
            Assert.False(string.IsNullOrEmpty(stored.PasswordHash));
            Assert.NotEqual(request.Password, stored.PasswordHash);

            Assert.DoesNotContain(factory.Logs, m => m.Contains(request.Password));
        }

        [Fact]
        public async Task RegisterAsync_RepeatedEmailDifferentCase_ReturnsError()
        {
            using var factory = await TestServiceFactory.CreateAsync();
            using var scope = factory.ServiceProvider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IRegistrationService>();

            var first = new RegisterRequest
            {
                Email = "user@example.com",
                UserName = "user1",
                Password = "Password123",
                Role = RoleNames.Student
            };

            var second = new RegisterRequest
            {
                Email = "User@Example.com",
                UserName = "user2",
                Password = "Password123",
                Role = RoleNames.Student
            };

            var r1 = await service.RegisterAsync(first);
            var r2 = await service.RegisterAsync(second);

            Assert.True(r1.Success);
            Assert.False(r2.Success);
            Assert.NotNull(r2.Errors);
            Assert.True(r2.Errors.ContainsKey("email") && r2.Errors["email"].Contains("AlreadyRegistered"));
        }

        [Fact]
        public async Task RegisterAsync_RepeatedUsername_ReturnsError()
        {
            using var factory = await TestServiceFactory.CreateAsync();
            using var scope = factory.ServiceProvider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IRegistrationService>();

            var first = new RegisterRequest
            {
                Email = "a1@example.com",
                UserName = "duplicate_user",
                Password = "Password123",
                Role = RoleNames.Student
            };

            var second = new RegisterRequest
            {
                Email = "a2@example.com",
                UserName = "duplicate_user",
                Password = "Password123",
                Role = RoleNames.Student
            };

            var r1 = await service.RegisterAsync(first);
            var r2 = await service.RegisterAsync(second);

            Assert.True(r1.Success);
            Assert.False(r2.Success);
            Assert.NotNull(r2.Errors);
            Assert.True(r2.Errors.ContainsKey("userName") && r2.Errors["userName"].Contains("AlreadyInUse"));
        }

        [Fact]
        public async Task RegisterAsync_AdministratorOrUnknownRole_ReturnsError()
        {
            using var factory = await TestServiceFactory.CreateAsync();
            using var scope = factory.ServiceProvider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IRegistrationService>();

            var adminReq = new RegisterRequest
            {
                Email = "admin@example.com",
                UserName = "admin_user",
                Password = "Password123",
                Role = RoleNames.Administrator
            };

            var unknownReq = new RegisterRequest
            {
                Email = "unknown@example.com",
                UserName = "unknown_user",
                Password = "Password123",
                Role = "SuperUser"
            };

            var rAdmin = await service.RegisterAsync(adminReq);
            var rUnknown = await service.RegisterAsync(unknownReq);

            Assert.False(rAdmin.Success);
            Assert.False(rUnknown.Success);
            Assert.NotNull(rAdmin.Errors);
            Assert.NotNull(rUnknown.Errors);
            Assert.True(rAdmin.Errors.ContainsKey("role") && rAdmin.Errors["role"].Contains("NotAllowed"));
            Assert.True(rUnknown.Errors.ContainsKey("role") && rUnknown.Errors["role"].Contains("NotAllowed"));
        }

        [Fact]
        public async Task RegisterAsync_PasswordStoredAsHash_PlainPasswordNotStored()
        {
            using var factory = await TestServiceFactory.CreateAsync();
            using var scope = factory.ServiceProvider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IRegistrationService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var request = new RegisterRequest
            {
                Email = "hash2@example.com",
                UserName = "hash_user2",
                Password = "SuperSecret123",
                Role = RoleNames.Student
            };

            var r = await service.RegisterAsync(request);
            Assert.True(r.Success);

            var stored = await userManager.FindByEmailAsync(request.Email);
            Assert.NotNull(stored);
            Assert.False(string.IsNullOrEmpty(stored.PasswordHash));
            Assert.NotEqual(request.Password, stored.PasswordHash);
        }

        [Fact]
        public async Task RegisterAsync_PasswordNotLogged_NoLogsContainPassword()
        {
            using var factory = await TestServiceFactory.CreateAsync();
            using var scope = factory.ServiceProvider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IRegistrationService>();

            var request = new RegisterRequest
            {
                Email = "nolog2@example.com",
                UserName = "nolog_user2",
                Password = "DoNotLogThis123",
                Role = RoleNames.Student
            };

            var r = await service.RegisterAsync(request);
            Assert.True(r.Success);

            Assert.DoesNotContain(factory.Logs, m => m.Contains(request.Password));
        }
    }
}
