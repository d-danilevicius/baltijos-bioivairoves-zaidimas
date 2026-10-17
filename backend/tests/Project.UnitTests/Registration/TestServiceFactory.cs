using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Project.Api.Data;
using Project.Api.Entities;
using Project.Api.Services;

namespace Project.UnitTests.Registration
{
    internal class InMemoryLoggerProvider : ILoggerProvider
    {
        private readonly List<string> _logs;
        public InMemoryLoggerProvider(List<string> logs) => _logs = logs;
        public ILogger CreateLogger(string categoryName) => new InMemoryLogger(_logs);
        public void Dispose() { }

        private class InMemoryLogger : ILogger
        {
            private readonly List<string> _logs;
            public InMemoryLogger(List<string> logs) => _logs = logs;
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                try
                {
                    var message = formatter(state, exception);
                    lock (_logs)
                    {
                        _logs.Add(message);
                    }
                }
                catch { }
            }
        }

        private class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }
    }

    internal class TestServiceFactory : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }
        public List<string> Logs { get; }

        private readonly ServiceProvider _sp;

        private TestServiceFactory(ServiceProvider sp, List<string> logs)
        {
            _sp = sp;
            ServiceProvider = sp;
            Logs = logs;
        }

        public static async Task<TestServiceFactory> CreateAsync()
        {
            var services = new ServiceCollection();

            // In-memory logging
            var logs = new List<string>();
            services.AddLogging(builder => builder.ClearProviders().AddProvider(new InMemoryLoggerProvider(logs)));

            // In-memory EF Core
            var dbName = Guid.NewGuid().ToString();
            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseInMemoryDatabase(databaseName: dbName));

            // Identity and application services
            services.AddProjectIdentity(new ConfigurationBuilder().Build());

            var sp = services.BuildServiceProvider();

            // Ensure database created and roles present
            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync(RoleNames.Student))
                    await roleManager.CreateAsync(new IdentityRole(RoleNames.Student));
                if (!await roleManager.RoleExistsAsync(RoleNames.Teacher))
                    await roleManager.CreateAsync(new IdentityRole(RoleNames.Teacher));
                if (!await roleManager.RoleExistsAsync(RoleNames.Administrator))
                    await roleManager.CreateAsync(new IdentityRole(RoleNames.Administrator));
            }

            var factory = new TestServiceFactory((ServiceProvider)sp, logs);

            return factory;
        }

        public void Dispose()
        {
            _sp?.Dispose();
        }
    }
}
