using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Project.IntegrationTests.Login;

namespace Project.IntegrationTests.AccessControl;

public class AccessControlApiFixture : LoginApiFixture
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.AddControllers().AddApplicationPart(typeof(PolicyTestController).Assembly);
        });
    }
}
