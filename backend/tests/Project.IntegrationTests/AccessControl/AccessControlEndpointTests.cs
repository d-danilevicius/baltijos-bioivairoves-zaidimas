using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Project.Api.Entities;
using Project.IntegrationTests.Login;
using Xunit;

namespace Project.IntegrationTests.AccessControl;

public class AccessControlEndpointTests : IClassFixture<AccessControlApiFixture>
{
    private readonly AccessControlApiFixture _fixture;

    public AccessControlEndpointTests(AccessControlApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData(RoleNames.Student, "/api/test-policies/student")]
    [InlineData(RoleNames.Teacher, "/api/test-policies/teacher")]
    [InlineData(RoleNames.Administrator, "/api/test-policies/administrator")]
    public async Task TestPolicyEndpoint_MatchingRole_Returns200(string userRole, string endpoint)
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

        await helper.CreateUserAsync(email, userName, password, role: userRole);

        var loginResponse = await helper.LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var response = await client.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(RoleNames.Student, "/api/test-policies/teacher")]
    [InlineData(RoleNames.Student, "/api/test-policies/administrator")]
    [InlineData(RoleNames.Teacher, "/api/test-policies/student")]
    [InlineData(RoleNames.Teacher, "/api/test-policies/administrator")]
    [InlineData(RoleNames.Administrator, "/api/test-policies/student")]
    [InlineData(RoleNames.Administrator, "/api/test-policies/teacher")]
    public async Task TestPolicyEndpoint_NonMatchingRole_Returns403WithAccessDenied(string userRole, string endpoint)
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

        await helper.CreateUserAsync(email, userName, password, role: userRole);

        var loginResponse = await helper.LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var response = await client.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var errors = json.RootElement.GetProperty("errors").GetProperty("");
        var values = errors.EnumerateArray().Select(x => x.GetString()).OfType<string>();
        Assert.Contains("AccessDenied", values);
        Assert.Equal("Prieiga uždrausta", json.RootElement.GetProperty("detail").GetString());
    }

    [Theory]
    [InlineData("/api/test-policies/student")]
    [InlineData("/api/test-policies/teacher")]
    [InlineData("/api/test-policies/administrator")]
    public async Task TestPolicyEndpoint_NoSession_Returns401(string endpoint)
    {
        var client = _fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
