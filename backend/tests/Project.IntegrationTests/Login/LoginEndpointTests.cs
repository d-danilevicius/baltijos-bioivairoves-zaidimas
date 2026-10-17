using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.Entities;
using Xunit;

namespace Project.IntegrationTests.Login;

public class LoginEndpointTests : IClassFixture<LoginApiFixture>
{
    private readonly LoginApiFixture _fixture;
    private readonly HttpClient _client;

    public LoginEndpointTests(LoginApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = new Uri("https://localhost") });
    }

    private static string Unique(string prefix) => prefix + Guid.NewGuid().ToString("N").Substring(0, 12);

    private async Task<string> CreateUserAsync(string email, string userName, string password, bool isBlocked = false)
    {
        using var scope = _fixture.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = new ApplicationUser { UserName = userName, Email = email };
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, RoleNames.Student);
        if (isBlocked)
        {
            var stored = await db.Users.SingleAsync(u => u.Email == email);
            stored.IsBlocked = true;
            await db.SaveChangesAsync();
        }
        return (await userManager.FindByEmailAsync(email))!.Id;
    }

    private async Task<HttpResponseMessage> LoginAsync(string email, string password)
    {
        var req = new { email, password };
        return await _client.PostAsJsonAsync("/api/auth/login", req);
    }

    private async Task<string> GetCsrfTokenAsync()
    {
        var tokenRes = await _client.GetAsync("/api/auth/csrf");
        tokenRes.EnsureSuccessStatusCode();
        var token = JsonDocument.Parse(await tokenRes.Content.ReadAsStringAsync()).RootElement.GetProperty("token").GetString();
        return token!;
    }

    private async Task<HttpResponseMessage> LogoutWithTokenAsync(string token)
    {
        var msg = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
        if (!string.IsNullOrEmpty(token)) msg.Headers.Add("X-CSRF-TOKEN", token);
        return await _client.SendAsync(msg);
    }

    [Fact]
    public async Task Login_WithCorrectData_Returns200AndUser()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        await CreateUserAsync(email, userName, password);

        var res = await LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var doc = JsonDocument.Parse(await res.Content.ReadAsStreamAsync());
        var root = doc.RootElement;
        Assert.True(root.TryGetProperty("id", out var idProp));
        Assert.True(root.TryGetProperty("userName", out var unProp));
        Assert.True(root.TryGetProperty("role", out var roleProp));
        Assert.Equal(userName, unProp.GetString());
        Assert.Equal("Student", roleProp.GetString());
    }

    [Fact]
    public async Task WrongPasswordAndUnknownEmail_Return401WithInvalidCredentials()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        await CreateUserAsync(email, userName, password);

        var r1 = await LoginAsync(email, "WrongPass!");
        Assert.Equal(HttpStatusCode.Unauthorized, r1.StatusCode);
        var pd1 = JsonDocument.Parse(await r1.Content.ReadAsStringAsync());
        Assert.True(pd1.RootElement.TryGetProperty("errors", out var errors1));

        var r2 = await LoginAsync("noone@example.com", "Whatever1!");
        Assert.Equal(HttpStatusCode.Unauthorized, r2.StatusCode);
        var pd2 = JsonDocument.Parse(await r2.Content.ReadAsStringAsync());
        Assert.True(pd2.RootElement.TryGetProperty("errors", out var errors2));

        Assert.Contains("InvalidCredentials", errors1.GetProperty("").EnumerateArray().Select(e => e.GetString()));
        Assert.Contains("InvalidCredentials", errors2.GetProperty("").EnumerateArray().Select(e => e.GetString()));
    }

    [Fact]
    public async Task BlockedUser_CorrectPasswordReturns403_WrongPasswordReturns401()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        await CreateUserAsync(email, userName, password, isBlocked: true);

        var r1 = await LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.Forbidden, r1.StatusCode);
        var pd1 = JsonDocument.Parse(await r1.Content.ReadAsStringAsync());
        Assert.Contains("AccountBlocked", pd1.RootElement.GetProperty("errors").GetProperty("").EnumerateArray().Select(e => e.GetString()));

        var r2 = await LoginAsync(email, "WrongPass!");
        Assert.Equal(HttpStatusCode.Unauthorized, r2.StatusCode);
    }

    [Fact]
    public async Task LoginSetsSessionCookie_WithSecurityFlags()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        await CreateUserAsync(email, userName, password);

        var r = await LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);

        Assert.True(r.Headers.TryGetValues("Set-Cookie", out var cookies));
        var cookie = string.Join(";", cookies);
        Assert.Contains("HttpOnly", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Secure", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("SameSite=Lax", cookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Logout_CsrfAndAuthenticationBehaviour()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        await CreateUserAsync(email, userName, password);

        var loginRes = await LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        // Attempt logout without CSRF token
        var logoutRes = await LogoutWithTokenAsync(token: null);
        Assert.Equal(HttpStatusCode.BadRequest, logoutRes.StatusCode);
        var pd = JsonDocument.Parse(await logoutRes.Content.ReadAsStringAsync());
        Assert.Contains("InvalidCsrfToken", pd.RootElement.GetProperty("errors").GetProperty("").EnumerateArray().Select(e => e.GetString()));

        // Get CSRF token and then logout
        var token = await GetCsrfTokenAsync();
        var logoutRes2 = await LogoutWithTokenAsync(token);
        Assert.Equal(HttpStatusCode.NoContent, logoutRes2.StatusCode);

        // Second logout after successful logout returns 401
        var logoutRes3 = await LogoutWithTokenAsync(token);
        Assert.Equal(HttpStatusCode.Unauthorized, logoutRes3.StatusCode);
    }

    [Fact]
    public async Task SessionExpires_AfterTwentyNineMinutes_LogoutSucceeds()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        await CreateUserAsync(email, userName, password);

        var loginRes = await LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var token = await GetCsrfTokenAsync();

        // Advance time by 29 minutes - still valid
        _fixture.TestTimeProvider.Advance(TimeSpan.FromMinutes(29));
        var logoutOk = await LogoutWithTokenAsync(token);
        Assert.Equal(HttpStatusCode.NoContent, logoutOk.StatusCode);
    }

    [Fact]
    public async Task SessionExpires_AfterThirtyOneMinutes_LogoutIsRejected()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        await CreateUserAsync(email, userName, password);

        var loginRes = await LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var token = await GetCsrfTokenAsync();

        // Advance to 31 minutes - expired
        _fixture.TestTimeProvider.Advance(TimeSpan.FromMinutes(31));
        var logoutExpired = await LogoutWithTokenAsync(token);
        Assert.Equal(HttpStatusCode.Unauthorized, logoutExpired.StatusCode);
    }

    [Fact]
    public async Task ActionLog_RecordsLoginFailedSuccessAndLogout_AndNoPasswordsInLogsOrActionLog()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        var userId = await CreateUserAsync(email, userName, password);

        // Failed login
        var r1 = await LoginAsync(email, "WrongPass!");
        Assert.Equal(HttpStatusCode.Unauthorized, r1.StatusCode);

        // Successful login
        var r2 = await LoginAsync(email, password);
        Assert.Equal(HttpStatusCode.OK, r2.StatusCode);

        // Get CSRF and logout
        var token = await GetCsrfTokenAsync();
        var logoutRes = await LogoutWithTokenAsync(token);
        Assert.Equal(HttpStatusCode.NoContent, logoutRes.StatusCode);

        // Check ActionLog entries in DB
        using (var scope = _fixture.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var actions = await db.ActionLogs.Where(a => a.UserId == userId).ToListAsync();
            Assert.Contains(actions, a => a.Action == "Login.Failed");
            Assert.Contains(actions, a => a.Action == "Login.Success");
            Assert.Contains(actions, a => a.Action == "Logout");

            // Ensure no passwords in ActionLog
            foreach (var a in actions)
            {
                Assert.DoesNotContain("WrongPass", a.Action, StringComparison.Ordinal);
                Assert.DoesNotContain(password, a.Action, StringComparison.Ordinal);
            }
        }

        // Check captured logs do not contain passwords
        foreach (var log in _fixture.Logs)
        {
            Assert.DoesNotContain("WrongPass", log, StringComparison.Ordinal);
            Assert.DoesNotContain(password, log, StringComparison.Ordinal);
        }
    }
}
