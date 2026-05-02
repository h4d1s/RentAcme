using Consul;
using ConsulIntegrationHelpers.HostedServices;
using ConsulIntegrationHelpers.Services;
using Diagnostics;
using EventBus;
using HealthChecks.UI.Client;
using Identity.Models;
using Inventory.Application.Infrastructure.Security;
using Inventory.Infrastructure.Grpc;
using Inventory.Infrastructure.Security;
using Logging;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
        var hcBuilder = services.AddHealthChecks();

        hcBuilder
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddNpgSql(
                configuration["ConnectionStrings:InventoryDbContext"] ?? throw new ArgumentNullException("InventoryDbContext string is not configured"),
                name: "InventoryDB-check",
                tags: new string[] { "inventorydb" });

        // Mass Transit
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());

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
                int.Parse(configuration["Consul:Service:Port"] ?? throw new ArgumentNullException("Consul port is not configured"))
            ));

        // DI

        // Grpc
        services.AddGrpc(cfg => cfg.EnableDetailedErrors = true);
        services.AddLogging(logging =>
        {
            logging.AddFilter("Grpc", Microsoft.Extensions.Logging.LogLevel.Debug);
        });

        // Logging
        services.AddLoggingSerilog(builder);

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
        this IApplicationBuilder app,
        IHostEnvironment environment,
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        IHostApplicationLifetime applicationLifetime)
    {
        // Auth
        app.UseAuthentication();
        app.UseAuthorization();

        // Health
        app.UseHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

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
