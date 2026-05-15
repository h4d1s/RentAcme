using Diagnostics;
using EventBus;
using GrpcIntegrationHelpers;
using HealthChecks.UI.Client;
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
using Microsoft.Extensions.Logging;
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
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddNpgSql(
                configuration["ConnectionStrings:ReservationDbContext"] ?? throw new ArgumentNullException("ReservationDbContext is not configured"),
                name: "ReservationDB-check",
                tags: new string[] { "reservationdb" });

        // Mass Transit
        services.AddMassTransit(x =>
        {
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

        // CORS
        services.AddCors(options =>
            options.AddPolicy(name: "RentAcmeOrigins", builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials())
        );

        // Logging
        services.AddLoggingSerilog(builder);

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
        this IApplicationBuilder app,
        IHostEnvironment environment,
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        IHostApplicationLifetime applicationLifetime)
    {
        // Health
        app.UseHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });

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
