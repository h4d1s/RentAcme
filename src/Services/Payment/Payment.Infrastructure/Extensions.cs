using Consul;
using ConsulIntegrationHelpers.HostedServices;
using ConsulIntegrationHelpers.Services;
using EventBus;
using EventBus.Constants;
using GrpcIntegrationHelpers;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payment.Application.Infrastructure.Services;
using Payment.Infrastructure.IntegrationEvents.EventHandling;
using Payment.Infrastructure.Services;
using Stripe;
using System.Reflection;

namespace Payment.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostBuilder builder)
    {
        // DI
        services.AddScoped<IPaymentGateway, StripePaymentGateway>();

        // Health
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());

        // Consul
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            var address = configuration["Consul:Address"] ?? throw new ArgumentNullException("Consul address is not configured");
            consulConfig.Address = new Uri(address);
        }));
        services.AddScoped<IConsulServiceDiscovery, ConsulServiceDiscovery>();
        services.AddHostedService<ConsulServiceRegistration>(provider =>
            new ConsulServiceRegistration(
                provider.GetRequiredService<IConsulClient>(),
                provider.GetRequiredService<ILogger<ConsulServiceRegistration>>(),
                configuration["Consul:Service:Host"] ?? throw new ArgumentNullException("Consul host is not configured"),
                configuration["Consul:Service:Name"] ?? throw new ArgumentNullException("Consul service name is not configured"),
                int.Parse(configuration["Consul:Service:Port"] ?? throw new ArgumentNullException("Consul port is not configured"))
            ));

        // Stripe
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

        // Mass Transit
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());
            x.AddConsumer<CreatePaymentIntentCommandIntegrationEventConsumer>()
                .Endpoint(e => e.Name = QueuesConsts.CreatePaymentIntentCommandQueueName);

            x.UsingRabbitMq((context, busFactoryConfigurator) =>
            {
                busFactoryConfigurator.Host(
                    configuration["RabbitMQ:Hostname"],
                    "/",
                    hostConfigurator =>
                    {
                        hostConfigurator.Username(configuration["RabbitMQ:Username"] ?? throw new ArgumentNullException("RabbitMQ username is not configured"));
                        hostConfigurator.Password(configuration["RabbitMQ:Password"] ?? throw new ArgumentNullException("RabbitMQ password is not configured"));
                    });
                busFactoryConfigurator.ConfigureEndpoints(context);
            });
        });

        // Event bus
        services.AddEventBus();

        // GrpcHelpers
        services.AddGrpcIntegrationHelpers(builder, configuration);

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
