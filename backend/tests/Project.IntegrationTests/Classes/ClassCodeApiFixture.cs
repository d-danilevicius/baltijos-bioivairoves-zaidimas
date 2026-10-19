using System.Collections.Concurrent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Project.Api.Services;
using Project.IntegrationTests.Login;

namespace Project.IntegrationTests.Classes;

public class ClassCodeApiFixture : LoginApiFixture
{
    public QueuedClassCodeGenerator Codes { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IClassCodeGenerator>();
            services.AddSingleton<IClassCodeGenerator>(Codes);
        });
    }
}

public sealed class QueuedClassCodeGenerator : IClassCodeGenerator
{
    private readonly ConcurrentQueue<string> _codes = new();
    private readonly ClassCodeGenerator _random = new();

    public void Enqueue(params string[] codes)
    {
        foreach (var code in codes)
        {
            _codes.Enqueue(code);
        }
    }

    public string Generate() => _codes.TryDequeue(out var code) ? code : _random.Generate();
}
