using Consul;
using ConsulIntegrationHelpers.HostedServices;
using ConsulIntegrationHelpers.Services;
using Diagnostics;
using EventBus;
using GrpcIntegrationHelpers;
using HealthChecks.UI.Client;
using Identity;
using Logging;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Reservation.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostBuilder builder)
    {
        // DI

        // Health
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddSqlServer(
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
                    });
                busFactoryConfigurator.ConfigureEndpoints(context);
            });
        });

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
                int.Parse(configuration["Consul:Service:Port"] ?? throw new ArgumentNullException("Consul service port is not configured"))
            ));

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
