using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Project.IntegrationTests.Login;

namespace Project.IntegrationTests.Classes;

internal sealed class ClassTestClient
{
    private readonly HttpClient _client;
    private readonly LoginTestHelper _helper;

    private ClassTestClient(HttpClient client, LoginTestHelper helper)
    {
        _client = client;
        _helper = helper;
    }

    public static ClassTestClient CreateAnonymous(LoginApiFixture fixture)
    {
        var client = fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        return new ClassTestClient(client, new LoginTestHelper(fixture, client));
    }

    public static async Task<ClassTestClient> LoginAsync(LoginApiFixture fixture, string role)
    {
        var testClient = CreateAnonymous(fixture);
        var email = LoginTestHelper.Unique("e") + "@example.com";
        const string password = "StrongPass123";
        await testClient._helper.CreateUserAsync(email, LoginTestHelper.Unique("u"), password, role: role);
        var response = await testClient._helper.LoginAsync(email, password);
        response.EnsureSuccessStatusCode();
        return testClient;
    }

    public Task<HttpResponseMessage> ListAsync() => _client.GetAsync("/api/classes");

    public Task<HttpResponseMessage> GetAsync(string id) => _client.GetAsync($"/api/classes/{id}");

    public Task<HttpResponseMessage> CreateAsync(string name) => SendAsync(HttpMethod.Post, "/api/classes", new { name });

    public Task<HttpResponseMessage> DeleteAsync(string id) => SendAsync(HttpMethod.Delete, $"/api/classes/{id}");

    public Task<HttpResponseMessage> ListTasksAsync(string id) => _client.GetAsync($"/api/classes/{id}/tasks");

    public Task<HttpResponseMessage> CreateGbifTasksAsync(string id, object body) =>
        SendAsync(HttpMethod.Post, $"/api/classes/{id}/gbif-tasks", body);

    public async Task<HttpResponseMessage[]> CreateGbifTasksAtTheSameTimeAsync(string id, object body, int count)
    {
        // One CSRF token is used for all requests, because every new token replaces the previous one
        var token = await _helper.GetCsrfTokenAsync();
        var requests = Enumerable.Range(0, count).Select(_ =>
        {
            var message = new HttpRequestMessage(HttpMethod.Post, $"/api/classes/{id}/gbif-tasks");
            message.Headers.Add("X-CSRF-TOKEN", token);
            message.Content = JsonContent.Create(body);
            return _client.SendAsync(message);
        });

        return await Task.WhenAll(requests);
    }

    public Task<HttpResponseMessage> JoinAsync(string code, bool confirmChange = false) =>
        SendAsync(HttpMethod.Post, "/api/classes/join", new { code, confirmChange });

    public async Task<HttpResponseMessage[]> JoinAtTheSameTimeAsync(string code, int count)
    {
        // One CSRF token is used for all requests, because every new token replaces the previous one
        var token = await _helper.GetCsrfTokenAsync();
        var requests = Enumerable.Range(0, count).Select(_ =>
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "/api/classes/join");
            message.Headers.Add("X-CSRF-TOKEN", token);
            message.Content = JsonContent.Create(new { code, confirmChange = false });
            return _client.SendAsync(message);
        });

        return await Task.WhenAll(requests);
    }

    public async Task<HttpResponseMessage> SendWithoutCsrfTokenAsync(HttpMethod method, string url, object? body = null)
    {
        var message = new HttpRequestMessage(method, url);
        if (body != null)
        {
            message.Content = JsonContent.Create(body);
        }

        return await _client.SendAsync(message);
    }

    public async Task<string> CreateAndGetIdAsync(string name)
    {
        var response = await CreateAsync(name);
        response.EnsureSuccessStatusCode();
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("id").GetString()!;
    }

    public async Task<string> CreateAndGetCodeAsync(string name)
    {
        var response = await CreateAsync(name);
        response.EnsureSuccessStatusCode();
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("code").GetString()!;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body = null)
    {
        var message = new HttpRequestMessage(method, url);
        message.Headers.Add("X-CSRF-TOKEN", await _helper.GetCsrfTokenAsync());
        if (body != null)
        {
            message.Content = JsonContent.Create(body);
        }

        return await _client.SendAsync(message);
    }
}
