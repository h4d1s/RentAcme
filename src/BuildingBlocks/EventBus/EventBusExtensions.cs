using EventBus.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
