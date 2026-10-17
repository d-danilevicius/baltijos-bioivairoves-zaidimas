using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Project.Api.Data;

namespace Project.IntegrationTests.Registration;

public class RegistrationApiFixture : WebApplicationFactory<Program>, Xunit.IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        var builder = new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres");

        _container = builder.Build();
        await _container.StartAsync();

        // Force host creation and apply migrations
        using var client = CreateClient();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            var dict = new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["ConnectionStrings:Default"] = _container.GetConnectionString()
            };
            cfg.AddInMemoryCollection(dict);
        });
    }

    async Task Xunit.IAsyncLifetime.DisposeAsync()
    {
        if (_container != null)
            await _container.StopAsync();

        base.Dispose();
    }
}
