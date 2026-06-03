using Identity.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Extensions.Http;

namespace Identity;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IHostBuilder builder,
        IConfiguration configuration)
    {
        services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
        });

        // DI
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IIdentityManagerService, IdentityManagerService>();
        services.AddHttpClient<IIdentityManagerService, IdentityManagerService>(config =>
        {
            config.BaseAddress = new Uri(configuration["Keycloak:BaseUrl"] ?? throw new ArgumentNullException("Keycloak:BaseUrl"));
        });

        services.AddScoped<IIdentityTokenService, IdentityTokenService>();
        services.AddHttpClient<IIdentityTokenService, IdentityTokenService>(config =>
        {
            config.BaseAddress = new Uri(configuration["Keycloak:BaseUrl"] ?? throw new ArgumentNullException("Keycloak:BaseUrl"));
        });
        services.AddHttpContextAccessor();

        return services;
    }
}