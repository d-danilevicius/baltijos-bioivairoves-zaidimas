using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Project.Api.Data;
using Project.Api.Entities;
using Xunit;

namespace Project.IntegrationTests.Registration;

public class RegistrationEndpointTests : IClassFixture<RegistrationApiFixture>
{
    private readonly RegistrationApiFixture _fixture;
    private readonly HttpClient _client;

    public RegistrationEndpointTests(RegistrationApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    private static string Unique(string prefix) => prefix + Guid.NewGuid().ToString("N").Substring(0, 12);

    [Fact]
    public async Task Post_Register_ValidRequest_Returns201AndStoresHash()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        var req = new { email, userName, password, role = "Student" };

        var res = await _client.PostAsJsonAsync("/api/auth/register", req);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var doc = await JsonDocument.ParseAsync(await res.Content.ReadAsStreamAsync());
        Assert.True(doc.RootElement.TryGetProperty("id", out var idProp));
        Assert.True(doc.RootElement.TryGetProperty("userName", out var unProp));
        Assert.True(doc.RootElement.TryGetProperty("role", out var roleProp));
        Assert.Equal(userName, unProp.GetString());
        Assert.Equal("Student", roleProp.GetString());

        // Response must not contain password or passwordHash
        Assert.False(doc.RootElement.TryGetProperty("password", out _));
        Assert.False(doc.RootElement.TryGetProperty("passwordHash", out _));

        // Verify stored hash in DB
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;

        using var db = new ApplicationDbContext(options);
        var stored = await db.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpperInvariant());
        Assert.NotNull(stored);
        Assert.False(string.IsNullOrEmpty(stored.PasswordHash));
        Assert.NotEqual(password, stored.PasswordHash);
    }

    [Fact]
    public async Task Post_Register_RepeatedEmailDifferentCase_Returns409()
    {
        var email = Unique("e") + "@example.com";
        var userName1 = Unique("u1");
        var userName2 = Unique("u2");
        var password = "StrongPass123";

        var req1 = new { email, userName = userName1, password, role = "Student" };
        var req2 = new { email = email.ToUpperInvariant(), userName = userName2, password, role = "Student" };

        var r1 = await _client.PostAsJsonAsync("/api/auth/register", req1);
        var r2 = await _client.PostAsJsonAsync("/api/auth/register", req2);

        Assert.Equal(HttpStatusCode.Created, r1.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, r2.StatusCode);

        var pd = JsonDocument.Parse(await r2.Content.ReadAsStringAsync());
        var root = pd.RootElement;
        Assert.True(root.TryGetProperty("errors", out var errors));
        Assert.True(errors.TryGetProperty("email", out var emailErrors));
        Assert.Contains("AlreadyRegistered", emailErrors.EnumerateArray().Select(e => e.GetString()));
    }

    [Fact]
    public async Task Post_Register_RepeatedUserName_Returns409()
    {
        var email1 = Unique("e1") + "@example.com";
        var email2 = Unique("e2") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        var req1 = new { email = email1, userName, password, role = "Student" };
        var req2 = new { email = email2, userName, password, role = "Student" };

        var r1 = await _client.PostAsJsonAsync("/api/auth/register", req1);
        var r2 = await _client.PostAsJsonAsync("/api/auth/register", req2);

        Assert.Equal(HttpStatusCode.Created, r1.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, r2.StatusCode);

        var pd = JsonDocument.Parse(await r2.Content.ReadAsStringAsync());
        var root = pd.RootElement;
        Assert.True(root.TryGetProperty("errors", out var errors));
        Assert.True(errors.TryGetProperty("userName", out var userErrors));
        Assert.Contains("AlreadyInUse", userErrors.EnumerateArray().Select(e => e.GetString()));
    }

    [Fact]
    public async Task Post_Register_AdministratorRole_Returns400()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        var req = new { email, userName, password, role = "Administrator" };
        var res = await _client.PostAsJsonAsync("/api/auth/register", req);
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var pd = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var root = pd.RootElement;
        Assert.True(root.TryGetProperty("errors", out var errors));
        Assert.True(errors.TryGetProperty("role", out var roleErrors));
        Assert.Contains("NotAllowed", roleErrors.EnumerateArray().Select(e => e.GetString()));
    }

    [Fact]
    public async Task Post_Register_ShortPassword_Returns400()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "short";

        var req = new { email, userName, password, role = "Student" };
        var res = await _client.PostAsJsonAsync("/api/auth/register", req);
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var pd = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var root = pd.RootElement;
        Assert.True(root.TryGetProperty("errors", out var errors));
        Assert.True(errors.TryGetProperty("password", out var passErrors));
        Assert.Contains("InvalidLength", passErrors.EnumerateArray().Select(e => e.GetString()));
    }

    [Fact]
    public async Task Post_Register_ConcurrentIdenticalRequests_One201One409()
    {
        var email = Unique("e") + "@example.com";
        var userName = Unique("u");
        var password = "StrongPass123";

        var req = new { email, userName, password, role = "Student" };

        var t1 = _client.PostAsJsonAsync("/api/auth/register", req);
        var t2 = _client.PostAsJsonAsync("/api/auth/register", req);

        var r1 = await t1;
        var r2 = await t2;

        var s = new[] { r1.StatusCode, r2.StatusCode };
        Assert.Contains(HttpStatusCode.Created, s);
        Assert.Contains(HttpStatusCode.Conflict, s);

        // If one returned 500, the test will report it when run locally
    }
}
