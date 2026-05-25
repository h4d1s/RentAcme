using Diagnostics;
using EventBus;
using GrpcIntegrationHelpers;
using Identity;
using Logging;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Reservation.Infrastructure.Security;

namespace Reservation.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostBuilder builder)
    {
        // DI

        // Security
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

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
            .AddRabbitMQ(sp => sp.GetRequiredService<IConnection>())
            .AddNpgSql(
                configuration["ConnectionStrings:ReservationDbContext"] ?? throw new ArgumentNullException("ReservationDbContext is not configured"),
                name: "ReservationDB-check",
                tags: new string[] { "reservationdb" });

        // Mass Transit
        services.AddMassTransit(x =>
        {
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

        // CORS
        services.AddCors(options =>
            options.AddPolicy(name: "RentAcmeOrigins", builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials())
        );

        // Logging
        services.AddLoggingSerilog("Reservation", builder, configuration);

        // Diagnostics
        services.AddObservability("Reservation", configuration);

        // Event Bus
        services.AddEventBus();

        // GrpcHelpers
        services.AddGrpcIntegrationHelpers(builder, configuration);

        // Identity
        services.AddIdentityServices(builder, configuration);

        return services;
    }

    public static IApplicationBuilder ConfigureInfrastructureServices(
        this WebApplication app,
        IHostEnvironment environment,
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        IHostApplicationLifetime applicationLifetime)
    {
        // Health
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });
        app.MapHealthChecks("/health/ready");

        // Auth
        app.UseAuthentication();
        app.UseAuthorization();

        // CORS
        app.UseCors("RentAcmeOrigins");

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
