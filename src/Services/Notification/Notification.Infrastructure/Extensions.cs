using Consul;
using ConsulIntegrationHelpers.HostedServices;
using ConsulIntegrationHelpers.Services;
using GrpcIntegrationHelpers;
using HealthChecks.UI.Client;
using Identity;
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
using Notification.Infrastructure.Services;
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
                int.Parse(configuration["Consul:Service:Port"] ?? throw new ArgumentNullException("Consul service port is not configured"))
            ));

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
                        hostConfigurator.Username(configuration["RabbitMQ:Username"] ?? throw new ArgumentNullException("RabbitMQ username is not configured"));
                        hostConfigurator.Password(configuration["RabbitMQ:Password"] ?? throw new ArgumentNullException("RabbitMQ password is not configured"));
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

    public static IEndpointRouteBuilder ConfigureEndpointApiServices(
        this IEndpointRouteBuilder app)
    {
        // Grpc
        app.MapGrpcService<NotificationGrpcServerService>();

        return app;
    }
}
