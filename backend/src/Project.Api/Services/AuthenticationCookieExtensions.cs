using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Project.Api.Services;

public static class AuthenticationCookieExtensions
{
    public static IServiceCollection AddAuthenticationCookieConfiguration(this IServiceCollection services)
    {
        // Ensure a TimeProvider is available and replaceable in tests
        services.TryAddSingleton<System.TimeProvider>(System.TimeProvider.System);

        // Configure cookie parameters for the Identity application scheme
        services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;

            // Default events to avoid redirects for API endpoints
            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = ctx =>
                {
                    if (IsApiRequest(ctx.Request))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return System.Threading.Tasks.Task.CompletedTask;
                    }

                    ctx.Response.Redirect(ctx.RedirectUri);
                    return System.Threading.Tasks.Task.CompletedTask;
                },
                OnRedirectToAccessDenied = ctx =>
                {
                    if (IsApiRequest(ctx.Request))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return System.Threading.Tasks.Task.CompletedTask;
                    }

                    ctx.Response.Redirect(ctx.RedirectUri);
                    return System.Threading.Tasks.Task.CompletedTask;
                }
            };
        });

        // Configure TimeProvider injection into CookieAuthenticationOptions via a named configurator
        services.AddSingleton<Microsoft.Extensions.Options.IConfigureOptions<CookieAuthenticationOptions>, CookieAuthenticationOptionsConfigurator>();

        return services;
    }

    private static bool IsApiRequest(HttpRequest req)
    {
        return req.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);
    }

    private class CookieAuthenticationOptionsConfigurator : Microsoft.Extensions.Options.IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        private readonly System.TimeProvider _timeProvider;

        public CookieAuthenticationOptionsConfigurator(System.TimeProvider timeProvider)
        {
            _timeProvider = timeProvider ?? System.TimeProvider.System;
        }

        public void Configure(string? name, CookieAuthenticationOptions options)
        {
            // Only configure the application scheme
            if (name is null)
                return;

            if (!string.Equals(name, IdentityConstants.ApplicationScheme, StringComparison.Ordinal))
                return;

            options.TimeProvider = _timeProvider;
        }

        public void Configure(CookieAuthenticationOptions options) => Configure(Options.DefaultName, options);
    }
}
