using Diagnostics;
using GrpcIntegrationHelpers;
using Identity;
using Logging;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notification.Appication.Infrastructure.Services;
using Notification.Infrastructure.Grpc;
using Notification.Infrastructure.Hubs;
using Notification.Infrastructure.Services;
using RabbitMQ.Client;
using System.Reflection;

namespace Notification.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostBuilder builder)
    {
        // DI
        services.AddTransient<IEmailService, EmailService>();

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

        // Mass Transit
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());
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

        // Grpc
        services.AddGrpc(cfg => cfg.EnableDetailedErrors = true);
        services.AddLogging(logging =>
        {
            logging.AddFilter("Grpc", Microsoft.Extensions.Logging.LogLevel.Debug);
        });

        // GrpcHelpers
        services.AddGrpcIntegrationHelpers(builder, configuration);

        // Identity
        services.AddIdentityServices(builder, configuration);

        // Logging
        services.AddLoggingSerilog("Notification", builder, configuration);

        // Diagnostics
        services.AddObservability("Notification", configuration);

        // SignalR
        services.AddSignalR();

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

        // SignalR
        app.MapHub<ReservationHub>("/reservationHub");

        return app;
    }

    public static IEndpointRouteBuilder ConfigureEndpointApiServices(
        this IEndpointRouteBuilder app)
    {
        // Grpc
        app.MapGrpcService<NotificationGrpcServerService>();

        return app;
    }
}
