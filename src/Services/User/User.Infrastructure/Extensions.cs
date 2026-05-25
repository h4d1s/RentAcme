using Diagnostics;
using EventBus;
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
using RabbitMQ.Client;
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
                configuration["ConnectionStrings:UserDbContext"] ?? throw new ArgumentNullException("User Db context is not configured"),
                name: "UserDB-check",
                tags: new string[] { "userdb" });
        
        // Security
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        // DI

        // Mass Transit
        services.AddMassTransit(x =>
        {
            x.AddConsumer<IdentityIntegrationEventConsumer>();

            x.UsingRabbitMq((context, busFactoryConfigurator) =>
            {
                var connectionString = configuration.GetConnectionString("RabbitMQ") ?? throw new ArgumentNullException("RabbitMQ connecting string is not configured");
                busFactoryConfigurator.Host(new Uri(connectionString),
                    hostConfigurator =>
                    {
                        hostConfigurator.RequestedConnectionTimeout(TimeSpan.FromSeconds(2));
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
        services.AddLoggingSerilog("User", builder, configuration);

        // Diagnostics
        services.AddObservability("User", configuration);

        // Event Bus
        services.AddEventBus();

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
