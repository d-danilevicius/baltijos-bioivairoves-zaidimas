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
    [InlineData(PolicyNames.Student, RoleNames.Student, true)]
    [InlineData(PolicyNames.Student, RoleNames.Teacher, false)]
    [InlineData(PolicyNames.Student, RoleNames.Administrator, false)]
    [InlineData(PolicyNames.Teacher, RoleNames.Student, false)]
    [InlineData(PolicyNames.Teacher, RoleNames.Teacher, true)]
    [InlineData(PolicyNames.Teacher, RoleNames.Administrator, false)]
    [InlineData(PolicyNames.Administrator, RoleNames.Student, false)]
    [InlineData(PolicyNames.Administrator, RoleNames.Teacher, false)]
    [InlineData(PolicyNames.Administrator, RoleNames.Administrator, true)]
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
    [InlineData(PolicyNames.Student)]
    [InlineData(PolicyNames.Teacher)]
    [InlineData(PolicyNames.Administrator)]
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
