using Microsoft.Extensions.DependencyInjection;

namespace Project.Api.Authorization;

public static class RolePolicyExtensions
{
    public static IServiceCollection AddRolePolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.Student, policy => policy.RequireRole(PolicyNames.Student));
            options.AddPolicy(PolicyNames.Teacher, policy => policy.RequireRole(PolicyNames.Teacher));
            options.AddPolicy(PolicyNames.Administrator, policy => policy.RequireRole(PolicyNames.Administrator));
        });

        return services;
    }
}
