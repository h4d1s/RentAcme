using EventBus.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus;

public static class EventBusExtensions
{
    public static IServiceCollection AddEventBus(
        this IServiceCollection services)
    {
        // DI
        services.AddScoped<IIntegrationEventService, IntegrationEventService>();

        return services;
    }
}
