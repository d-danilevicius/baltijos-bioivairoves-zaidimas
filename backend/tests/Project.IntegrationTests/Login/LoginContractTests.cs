using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Project.IntegrationTests.Login;

public class LoginContractTests : IClassFixture<LoginApiFixture>
{
    private readonly LoginTestHelper _helper;

    public LoginContractTests(LoginApiFixture fixture)
    {
        var client = fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        _helper = new LoginTestHelper(fixture, client);
    }

    [Fact]
    public async Task Login_ScenarioEmptyEmailAndPassword_Returns400WithRequiredErrors()
    {
        var response = await _helper.LoginAsync(string.Empty, string.Empty);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var errors = json.RootElement.GetProperty("errors");

        var emailErrors = errors.GetProperty("email").EnumerateArray().Select(x => x.GetString()).OfType<string>();
        var passwordErrors = errors.GetProperty("password").EnumerateArray().Select(x => x.GetString()).OfType<string>();

        Assert.Contains("Required", emailErrors);
        Assert.Contains("Required", passwordErrors);
    }

    [Fact]
    public async Task Logout_ScenarioSuccessfulLoginAndValidCsrf_RemovesCookieAndProtectedRequestReturns401()
    {
        var email = LoginTestHelper.Unique("e") + "@example.com";
        var userName = LoginTestHelper.Unique("u");
        var password = "StrongPass123";

        await _helper.CreateUserAsync(email, userName, password);

        var loginResponse = await _helper.LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var csrfToken = await _helper.GetCsrfTokenAsync();
        var logoutResponse = await _helper.LogoutWithTokenAsync(csrfToken);

        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);
        Assert.True(logoutResponse.Headers.TryGetValues("Set-Cookie", out var setCookies));

        var authCookieHeader = setCookies.Single(h => h.Contains(".AspNetCore.Identity.Application=", StringComparison.Ordinal));
        Assert.Contains("Expires=", authCookieHeader, StringComparison.OrdinalIgnoreCase);

        var expiresValue = authCookieHeader
            .Split(';', StringSplitOptions.TrimEntries)
            .First(part => part.StartsWith("Expires=", StringComparison.OrdinalIgnoreCase))
            .Substring("Expires=".Length);

        var expires = DateTimeOffset.Parse(expiresValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        Assert.True(expires <= DateTimeOffset.UtcNow);

        var csrfTokenAfterLogout = await _helper.GetCsrfTokenAsync();
        var protectedResponse = await _helper.LogoutWithTokenAsync(csrfTokenAfterLogout);

        Assert.Equal(HttpStatusCode.Unauthorized, protectedResponse.StatusCode);
    }
}
