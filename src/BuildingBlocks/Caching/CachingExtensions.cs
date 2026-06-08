using Caching.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Caching;

public static class CachingExtensions
{
    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redis = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Connection string 'Redis' not found.");
            return ConnectionMultiplexer.Connect(redis);
        });
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
