using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Authorization;
using Project.Api.Data;
using Project.Api.Entities;

namespace Project.Api.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password length according to spec
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            // Unique email
            options.User.RequireUniqueEmail = true;

            // Allowed username characters: letters, digits and underscore
            options.User.AllowedUserNameCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configure authentication cookie and TimeProvider for tests
        services.AddAuthenticationCookieConfiguration();

        // Role policies: Student, Teacher, Administrator
        services.AddRolePolicies();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IActionLogWriter, ActionLogWriter>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddSingleton<IClassCodeGenerator, ClassCodeGenerator>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<IClassMembershipService, ClassMembershipService>();
        services.AddSingleton(Random.Shared);
        services.AddSingleton<GbifTaskFactory>();
        services.AddSingleton<SeasonalTaskFactory>();
        services.AddScoped<IGbifTaskService, GbifTaskService>();
        services.AddScoped<IClassTaskService, ClassTaskService>();
        services.AddScoped<ITeacherTaskService, TeacherTaskService>();
        services.AddScoped<IStudentTaskService, StudentTaskService>();
        services.AddScoped<IStudentTaskDetailsService, StudentTaskDetailsService>();

        return services;
    }
}
