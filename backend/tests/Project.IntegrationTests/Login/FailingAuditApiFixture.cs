using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Project.Api.Services;

namespace Project.IntegrationTests.Login;

public class FailingAuditApiFixture : LoginApiFixture
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IActionLogWriter>();
            services.AddScoped<IActionLogWriter, FailingActionLogWriter>();
        });
    }

    private sealed class FailingActionLogWriter : IActionLogWriter
    {
        public Task WriteAsync(string? userId, string action, DateTime occurredAtUtc, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("Audit write failed");
    }
}
