using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Project.Api.Entities;
using Project.IntegrationTests.Login;
using Xunit;

namespace Project.IntegrationTests.AccessControl;

public class AuthMeEndpointTests : IClassFixture<AccessControlApiFixture>
{
    private readonly AccessControlApiFixture _fixture;

    public AuthMeEndpointTests(AccessControlApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAuthMe_ValidSession_Returns200WithCurrentUser()
    {
        var client = _fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        var helper = new LoginTestHelper(_fixture, client);
        var email = LoginTestHelper.Unique("e") + "@example.com";
        var userName = LoginTestHelper.Unique("u");
        const string password = "StrongPass123";

        var userId = await helper.CreateUserAsync(email, userName, password, role: RoleNames.Teacher);

        var loginResponse = await helper.LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = json.RootElement;

        Assert.Equal(userId, root.GetProperty("id").GetString());
        Assert.Equal(userName, root.GetProperty("userName").GetString());
        Assert.Equal(RoleNames.Teacher, root.GetProperty("role").GetString());
    }

    [Fact]
    public async Task GetAuthMe_NoSession_Returns401()
    {
        var client = _fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthMe_SessionExpiredAfterThirtyOneMinutes_Returns401()
    {
        var client = _fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        var helper = new LoginTestHelper(_fixture, client);
        var email = LoginTestHelper.Unique("e") + "@example.com";
        var userName = LoginTestHelper.Unique("u");
        const string password = "StrongPass123";

        await helper.CreateUserAsync(email, userName, password, role: RoleNames.Student);

        var loginResponse = await helper.LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        _fixture.TestTimeProvider.Advance(TimeSpan.FromMinutes(31));

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
