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
                        hostConfigurator.RequestedConnectionTimeout(TimeSpan.FromSeconds(2));
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
