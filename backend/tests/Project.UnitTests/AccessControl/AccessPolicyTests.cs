using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Authorization;
using Project.Api.Entities;
using Project.UnitTests.Registration;
using Xunit;

namespace Project.UnitTests.AccessControl;

public class AccessPolicyTests
{
    [Theory]
    [InlineData(PolicyName.Student, RoleName.Student, true)]
    [InlineData(PolicyName.Student, RoleName.Teacher, false)]
    [InlineData(PolicyName.Student, RoleName.Administrator, false)]
    [InlineData(PolicyName.Teacher, RoleName.Student, false)]
    [InlineData(PolicyName.Teacher, RoleName.Teacher, true)]
    [InlineData(PolicyName.Teacher, RoleName.Administrator, false)]
    [InlineData(PolicyName.Administrator, RoleName.Student, false)]
    [InlineData(PolicyName.Administrator, RoleName.Teacher, false)]
    [InlineData(PolicyName.Administrator, RoleName.Administrator, true)]
    public async Task AuthorizeAsync_RoleAgainstPolicyMatrix_ReturnsExpectedResult(string policyName, string roleName, bool expectedSuccess)
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();

        var authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateAuthenticatedUser(roleName);

        var result = await authorizationService.AuthorizeAsync(user, resource: null, policyName);

        Assert.Equal(expectedSuccess, result.Succeeded);
    }

    [Theory]
    [InlineData(PolicyName.Student)]
    [InlineData(PolicyName.Teacher)]
    [InlineData(PolicyName.Administrator)]
    public async Task AuthorizeAsync_AnonymousUserAgainstAnyPolicy_ReturnsForbidden(string policyName)
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();

        var authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await authorizationService.AuthorizeAsync(anonymousUser, resource: null, policyName);

        Assert.False(result.Succeeded);
    }

    private static ClaimsPrincipal CreateAuthenticatedUser(string roleName)
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "test-user"),
                new Claim(ClaimTypes.Role, roleName)
            ],
            authenticationType: "TestAuth");

        return new ClaimsPrincipal(identity);
    }
}
