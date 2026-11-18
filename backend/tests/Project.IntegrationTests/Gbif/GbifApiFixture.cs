using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Project.Api.Integrations.Gbif;
using Project.IntegrationTests.Login;

namespace Project.IntegrationTests.Gbif;

public class GbifApiFixture : LoginApiFixture
{
    public FakeGbifClient Gbif { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IGbifClient>();
            services.AddSingleton<IGbifClient>(Gbif);
        });
    }
}
