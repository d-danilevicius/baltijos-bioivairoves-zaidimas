using Microsoft.Extensions.Options;

namespace Project.Api.Integrations.Gbif;

public static class GbifServiceCollectionExtensions
{
    public static IServiceCollection AddGbifClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GbifOptions>()
            .Bind(configuration.GetSection(GbifOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<IGbifClient, GbifClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<GbifOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}
