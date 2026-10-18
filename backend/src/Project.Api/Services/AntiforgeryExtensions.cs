using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Api.Services;

public static class AntiforgeryExtensions
{
    public static IServiceCollection AddCsrfProtection(this IServiceCollection services)
    {
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
        });

        // Register the global antiforgery authorization filter (validates CSRF for unsafe methods for authenticated users)
        services.Configure<MvcOptions>(opts =>
        {
            // Register the filter type so it is constructed by DI
            opts.Filters.Add(typeof(AntiforgeryValidationFilter));
        });

        return services;
    }
}
