using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Project.IntegrationTests.Registration;
using Xunit;

namespace Project.IntegrationTests.OpenApi;

public class OpenApiProductionTests : IClassFixture<RegistrationApiFixture>
{
    private readonly RegistrationApiFixture _fixture;

    public OpenApiProductionTests(RegistrationApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetOpenApiDocument_InProduction_ReturnsNotFound()
    {
        var client = CreateProductionClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDocumentationPage_InProduction_ReturnsNotFound()
    {
        var client = CreateProductionClient();

        var response = await client.GetAsync("/scalar");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private HttpClient CreateProductionClient()
    {
        return _fixture
            .WithWebHostBuilder(builder => builder.UseEnvironment("Production"))
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost")
            });
    }
}
