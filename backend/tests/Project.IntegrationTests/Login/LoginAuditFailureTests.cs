using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Project.IntegrationTests.Login;

public class LoginAuditFailureTests : IClassFixture<FailingAuditApiFixture>
{
    private readonly LoginTestHelper _helper;

    public LoginAuditFailureTests(FailingAuditApiFixture fixture)
    {
        var client = fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = new Uri("https://localhost") });
        _helper = new LoginTestHelper(fixture, client);
    }

    [Fact]
    public async Task Login_WhenAuditWriteFails_Returns500WithoutAuthCookie()
    {
        var email = LoginTestHelper.Unique("e") + "@example.com";
        var password = "StrongPass123";
        await _helper.CreateUserAsync(email, LoginTestHelper.Unique("u"), password);

        var response = await _helper.LoginAsync(email, password);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }
}
