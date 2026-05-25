using Diagnostics;
using EventBus;
using EventBus.Constants;
using Inventory.Infrastructure.Grpc;
using Inventory.Infrastructure.IntegrationEvents.EventHandling;
using Inventory.Infrastructure.Security;
using Logging;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using Serilog;
using System.Reflection;
using System.Security.Claims;

namespace Inventory.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostBuilder builder)
    {
        // Auth
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = configuration["Keycloak:Authority"];
                options.Audience = configuration["Keycloak:Audience"];
                options.MetadataAddress = configuration["Keycloak:MetadataAddress"] ?? throw new ArgumentNullException("Keycloak metadataAddress is not configured");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["Keycloak:Issuer"],
                    RoleClaimType = ClaimTypes.Role
                };
            });
        services.AddAuthorization();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

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
                configuration["ConnectionStrings:InventoryDbContext"] ?? throw new ArgumentNullException("InventoryDbContext string is not configured"),
                name: "InventoryDB-check",
                tags: new string[] { "inventorydb" });

        // Mass Transit
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());
            x.AddConsumer<LockVehicleCommandIntegrationEventConsumer>()
                .Endpoint(e => e.Name = QueuesConsts.VehicleLockCommandQueueName);
            x.AddConsumer<UnlockVehicleCommandIntegrationEventConsumer>()
                .Endpoint(e => e.Name = QueuesConsts.UnlockVehicleCommandQueueName);

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

        // DI

        // Grpc
        services.AddGrpc(cfg => cfg.EnableDetailedErrors = true);
        services.AddLogging(logging =>
        {
            logging.AddFilter("Grpc", Microsoft.Extensions.Logging.LogLevel.Debug);
        });

        // Logging
        services.AddLoggingSerilog("Inventory", builder, configuration);

        // Diagnostics
        services.AddObservability("Inventory", configuration);

        // CORS
        services.AddCors(options =>
            options.AddPolicy(name: "RentAcmeOrigins", builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials())
        );

        // Event Bus
        services.AddEventBus();

        return services;
    }

    public static IApplicationBuilder ConfigureInfrastructureServices(
        this WebApplication app,
        IHostEnvironment environment,
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        IHostApplicationLifetime applicationLifetime)
    {
        // Auth
        app.UseAuthentication();
        app.UseAuthorization();

        // Health
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });
        app.MapHealthChecks("/health/ready");

        // CORS
        app.UseCors("RentAcmeOrigins");

        // Serilog
        app.UseSerilogRequestLogging();

        return app;
    }

    public static IEndpointRouteBuilder ConfigureEndpointApiServices(
        this IEndpointRouteBuilder app)
    {
        // Grpc
        app.MapGrpcService<InventoryGrpcServerService>();

        return app;
    }
}
