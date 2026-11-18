using Microsoft.Extensions.DependencyInjection;

namespace Project.Api.Authorization;

public static class RolePolicyExtensions
{
    public static IServiceCollection AddRolePolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyName.Student, policy => policy.RequireRole(PolicyName.Student));
            options.AddPolicy(PolicyName.Teacher, policy => policy.RequireRole(PolicyName.Teacher));
            options.AddPolicy(PolicyName.Administrator, policy => policy.RequireRole(PolicyName.Administrator));
        });

        return services;
    }
}
