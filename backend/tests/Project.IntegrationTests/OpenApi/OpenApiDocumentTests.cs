using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Project.IntegrationTests.Registration;
using Xunit;

namespace Project.IntegrationTests.OpenApi;

public class OpenApiDocumentTests : IClassFixture<RegistrationApiFixture>
{
    private const string DocumentPath = "/openapi/v1.json";

    private readonly RegistrationApiFixture _fixture;

    public OpenApiDocumentTests(RegistrationApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetOpenApiDocument_InDevelopment_ReturnsOk()
    {
        var client = CreateClient();

        var response = await client.GetAsync(DocumentPath);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/auth/register", "post")]
    [InlineData("/api/auth/login", "post")]
    [InlineData("/api/auth/logout", "post")]
    [InlineData("/api/auth/csrf", "get")]
    [InlineData("/api/auth/me", "get")]
    [InlineData("/api/health", "get")]
    public async Task GetOpenApiDocument_InDevelopment_ContainsAllExistingEndpointPaths(string path, string method)
    {
        using var document = await GetDocumentAsync();

        var paths = document.RootElement.GetProperty("paths");

        Assert.True(paths.TryGetProperty(path, out var pathItem), $"Path {path} is missing from the OpenAPI document.");
        Assert.True(pathItem.TryGetProperty(method, out _), $"Operation {method.ToUpperInvariant()} {path} is missing from the OpenAPI document.");
    }

    [Theory]
    [InlineData("/api/auth/logout", "post")]
    [InlineData("/api/auth/me", "get")]
    public async Task GetOpenApiDocument_InDevelopment_DocumentsUnauthorizedStatusCode(string path, string method)
    {
        using var document = await GetDocumentAsync();

        var operation = document.RootElement.GetProperty("paths").GetProperty(path).GetProperty(method);
        var responses = operation.GetProperty("responses");

        Assert.True(responses.TryGetProperty("401", out _), $"Status code 401 is not documented for {method.ToUpperInvariant()} {path}.");
    }

    [Fact]
    public async Task GetOpenApiDocument_InDevelopment_DocumentsBadRequestForLogout()
    {
        using var document = await GetDocumentAsync();

        var responses = document.RootElement.GetProperty("paths").GetProperty("/api/auth/logout").GetProperty("post").GetProperty("responses");

        Assert.True(responses.TryGetProperty("400", out var badRequest), "Status code 400 is not documented for POST /api/auth/logout.");
        Assert.True(badRequest.TryGetProperty("content", out _), "Status code 400 of POST /api/auth/logout has no content.");
    }

    [Fact]
    public async Task GetOpenApiDocument_InDevelopment_DocumentsCsrfTokenResponseSchema()
    {
        await AssertOkResponseSchemaHasPropertyAsync("/api/auth/csrf", "token");
    }

    [Fact]
    public async Task GetOpenApiDocument_InDevelopment_DocumentsHealthResponseSchema()
    {
        await AssertOkResponseSchemaHasPropertyAsync("/api/health", "status");
    }

    [Theory]
    [InlineData("/api/auth/register", "post", "400")]
    [InlineData("/api/auth/register", "post", "409")]
    [InlineData("/api/auth/login", "post", "400")]
    [InlineData("/api/auth/login", "post", "401")]
    public async Task GetOpenApiDocument_InDevelopment_DocumentsErrorStatusCodes(string path, string method, string statusCode)
    {
        using var document = await GetDocumentAsync();

        var responses = document.RootElement.GetProperty("paths").GetProperty(path).GetProperty(method).GetProperty("responses");

        Assert.True(responses.TryGetProperty(statusCode, out _), $"Status code {statusCode} is not documented for {method.ToUpperInvariant()} {path}.");
    }

    private async Task AssertOkResponseSchemaHasPropertyAsync(string path, string propertyName)
    {
        using var document = await GetDocumentAsync();
        var root = document.RootElement;

        var responses = root.GetProperty("paths").GetProperty(path).GetProperty("get").GetProperty("responses");
        Assert.True(responses.TryGetProperty("200", out var ok), $"Status code 200 is not documented for GET {path}.");
        Assert.True(ok.TryGetProperty("content", out var content), $"Status code 200 of GET {path} has no content.");
        Assert.True(content.TryGetProperty("application/json", out var json), $"Status code 200 of GET {path} has no JSON content.");
        Assert.True(json.TryGetProperty("schema", out var schema), $"Status code 200 of GET {path} has no JSON schema.");

        if (schema.TryGetProperty("$ref", out var reference))
        {
            var name = reference.GetString()!.Split('/')[^1];
            Assert.True(root.GetProperty("components").GetProperty("schemas").TryGetProperty(name, out schema), $"Schema {name} is missing from components/schemas.");
        }

        Assert.True(schema.TryGetProperty("properties", out var properties) && properties.TryGetProperty(propertyName, out _), $"Response schema of GET {path} has no property {propertyName}.");
    }

    private HttpClient CreateClient()
    {
        return _fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
    }

    private async Task<JsonDocument> GetDocumentAsync()
    {
        var response = await CreateClient().GetAsync(DocumentPath);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }
}
