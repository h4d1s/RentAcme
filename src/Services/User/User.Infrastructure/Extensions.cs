using Consul;
using ConsulIntegrationHelpers.HostedServices;
using ConsulIntegrationHelpers.Services;
using Diagnostics;
using EventBus;
using HealthChecks.UI.Client;
using Identity;
using Logging;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using User.Infrastructure.Grpc;
using User.Infrastructure.IntegrationEvents.EventHandling;
using User.Infrastructure.Security;

namespace User.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostBuilder builder)
    {
        // Health check
        var hcBuilder = services.AddHealthChecks();
        hcBuilder
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddNpgSql(
                configuration["ConnectionStrings:UserDbContext"] ?? throw new ArgumentNullException("User Db context is not configured"),
                name: "UserDB-check",
                tags: new string[] { "userdb" });
        
        // Security
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

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

        // DI

        // Mass Transit
        services.AddMassTransit(x =>
        {
            x.AddConsumer<IdentityIntegrationEventConsumer>();

            x.UsingRabbitMq((context, busFactoryConfigurator) =>
            {
                busFactoryConfigurator.Host(
                    configuration["RabbitMQ:Hostname"],
                    "/",
                    hostConfigurator =>
                    {
                        hostConfigurator.Username(
                            configuration["RabbitMQ:Username"] ??
                            throw new ArgumentNullException("RabbitMQ Username is not configured"));
                        hostConfigurator.Password(
                            configuration["RabbitMQ:Password"] ??
                            throw new ArgumentNullException("RabbitMQ Password is not configured"));
                    });
                busFactoryConfigurator.ReceiveEndpoint("keycloak-client-event-queue", e =>
                {
                    e.Bind("amq.topic", s =>
                    {
                        s.RoutingKey = "KK.EVENT.CLIENT.rent-acme.#";
                        s.ExchangeType = "topic";
                    });

                    e.UseRawJsonDeserializer();
                    e.ConfigureConsumer<IdentityIntegrationEventConsumer>(context);
                });
            });
        });

        // Grpc
        services.AddGrpc(cfg => cfg.EnableDetailedErrors = true);
        services.AddLogging(logging =>
        {
            logging.AddFilter("Grpc", Microsoft.Extensions.Logging.LogLevel.Debug);
        });

        // CORS
        services.AddCors(options =>
            options.AddPolicy(name: "RentAcmeOrigins", builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials())
        );

        // Serilog
        services.AddLoggingSerilog(builder);

        // Diagnostics
        services.AddObservability("User", configuration);

        // Event Bus
        services.AddEventBus();

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
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // CORS
        app.UseCors("RentAcmeOrigins");

        return app;
    }

    public static IEndpointRouteBuilder ConfigureEndpointApiServices(
        this IEndpointRouteBuilder app)
    {
        // Grpc
        app.MapGrpcService<UserGrpcServerService>();

        return app;
    }
}
