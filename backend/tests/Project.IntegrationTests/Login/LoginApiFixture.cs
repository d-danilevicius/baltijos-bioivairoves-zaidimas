using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Project.IntegrationTests.Registration;

namespace Project.IntegrationTests.Login;

public class LoginApiFixture : RegistrationApiFixture
{
    public TestTimeProvider TestTimeProvider { get; } = new TestTimeProvider();
    public List<string> Logs { get; } = new List<string>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            // Replace or add the TimeProvider singleton so production code can consume it
            services.AddSingleton<System.TimeProvider>(TestTimeProvider);

            // In-memory logging provider
            services.AddLogging(b => b.ClearProviders().AddProvider(new InMemoryLoggerProvider(Logs)));
        });
    }
}
