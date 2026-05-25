using Diagnostics;
using EventBus;
using EventBus.Constants;
using GrpcIntegrationHelpers;
using Logging;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Payment.Application.Infrastructure.Services;
using Payment.Infrastructure.IntegrationEvents.EventHandling;
using Payment.Infrastructure.Services;
using RabbitMQ.Client;
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
        services.AddSingleton<IConnection>(sp =>
        {
            var connectionString = configuration.GetConnectionString("RabbitMQ")
                ?? throw new ArgumentNullException("RabbitMQ connection string is not configured");
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString),
            };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        var hcBuilder = services.AddHealthChecks();
        hcBuilder
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddRabbitMQ(sp => sp.GetRequiredService<IConnection>());

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
                var connectionString = configuration.GetConnectionString("RabbitMQ") ?? throw new ArgumentNullException("RabbitMQ connecting string is not configured");
                busFactoryConfigurator.Host(new Uri(connectionString),
                    hostConfigurator =>
                    {
                        hostConfigurator.RequestedConnectionTimeout(TimeSpan.FromSeconds(2));
                    });
                busFactoryConfigurator.ConfigureEndpoints(context);
            });
        });

        // Event bus
        services.AddEventBus();

        // Logging
        services.AddLoggingSerilog("Payment", builder, configuration);

        // Diagnostics
        services.AddObservability("Payment", configuration);

        // GrpcHelpers
        services.AddGrpcIntegrationHelpers(builder, configuration);

        return services;
    }

    public static IApplicationBuilder ConfigureInfrastructureServices(
        this WebApplication app,
        IHostEnvironment environment,
        IServiceProvider serviceProvider)
    {
        // Health
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });
        app.MapHealthChecks("/health/ready");

        return app;
    }
}
