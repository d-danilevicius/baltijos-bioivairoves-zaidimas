using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.Entities;

namespace Project.IntegrationTests.Login;

internal sealed class LoginTestHelper
{
    private readonly LoginApiFixture _fixture;
    private readonly HttpClient _client;

    public LoginTestHelper(LoginApiFixture fixture, HttpClient client)
    {
        _fixture = fixture;
        _client = client;
    }

    public static string Unique(string prefix) => prefix + Guid.NewGuid().ToString("N").Substring(0, 12);

    public async Task<string> CreateUserAsync(string email, string userName, string password, bool isBlocked = false)
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

    public Task<HttpResponseMessage> LoginAsync(string email, string password)
    {
        var req = new { email, password };
        return _client.PostAsJsonAsync("/api/auth/login", req);
    }

    public async Task<string> GetCsrfTokenAsync()
    {
        var tokenRes = await _client.GetAsync("/api/auth/csrf");
        tokenRes.EnsureSuccessStatusCode();
        var token = JsonDocument.Parse(await tokenRes.Content.ReadAsStringAsync()).RootElement.GetProperty("token").GetString();
        return token!;
    }

    public Task<HttpResponseMessage> LogoutWithTokenAsync(string token)
    {
        var msg = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
        if (!string.IsNullOrEmpty(token))
        {
            msg.Headers.Add("X-CSRF-TOKEN", token);
        }

        return _client.SendAsync(msg);
    }
}
