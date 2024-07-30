using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Stripe;
using MassTransit;
using Payment.Infrastructure.IntegrationEvents.EventHandling;
using Payment.Infrastructure.Services;
using System.Reflection;
using EventBus.Constants;
using Payment.Application.Infrastructure.Services;
using EventBus;
using ConsulIntegrationHelpers.HostedServices;
using ConsulIntegrationHelpers.Services;
using GrpcIntegrationHelpers;

namespace Payment.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DI
        services.AddScoped<IPaymentGateway, StripePaymentGateway>();

        // Health
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());

        // Consul
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            var address = configuration["Consul:Address"] ?? "";
            consulConfig.Address = new Uri(address);
        }));
        services.AddScoped<IConsulServiceDiscovery, ConsulServiceDiscovery>();
        services.AddHostedService<ConsulServiceRegistration>(provider =>
            new ConsulServiceRegistration(
                provider.GetRequiredService<IConsulClient>(),
                provider.GetRequiredService<ILogger<ConsulServiceRegistration>>(),
                configuration["Consul:Service:Host"],
                configuration["Consul:Service:Name"],
                int.Parse(configuration["Consul:Service:Port"])
            ));

        // Stripe
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        // Mass Transit
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());
            x.UsingRabbitMq((context, busFactoryConfigurator) =>
            {
                busFactoryConfigurator.Host(
                    configuration["RabbitMQ:Hostname"],
                    "/",
                    hostConfigurator => {
                        hostConfigurator.Username(configuration["RabbitMQ:Username"]);
                        hostConfigurator.Password(configuration["RabbitMQ:Password"]);
                    });
                busFactoryConfigurator.ConfigureEndpoints(context);
            });
        });

        // Event bus
        services.AddEventBus();

        // GrpcHelpers
        services.AddGrpcIntegrationHelpers();

        return services;
    }

    public static IApplicationBuilder ConfigureInfrastructureServices(
        this IApplicationBuilder app,
        IHostEnvironment environment,
        IServiceProvider serviceProvider)
    {
        // Health
        app.UseHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });

        return app;
    }
}
