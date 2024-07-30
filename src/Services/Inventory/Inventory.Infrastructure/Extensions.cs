using Inventory.Application.Infrastructure.IntegrationEvents;
using Inventory.Infrastructure.Grpc;
using Inventory.Infrastructure.IntegrationEvents;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation.AspNetCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Diagnostics;
using Microsoft.AspNetCore.Routing;
using ConsulIntegrationHelpers;
using Consul;
using EventBus;
using EventBus.Services;
using System.Reflection;
using EventBus.Constants;
using Inventory.Infrastructure.IntegrationEvents.EventHandling;
using ConsulIntegrationHelpers.Services;
using ConsulIntegrationHelpers.HostedServices;

namespace Inventory.Infrastructure;

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
            .AddSqlServer(
                configuration["ConnectionStrings:InventoryDbContext"],
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
                    hostConfigurator => {
                        hostConfigurator.Username(configuration["RabbitMQ:Username"]);
                        hostConfigurator.Password(configuration["RabbitMQ:Password"]);
                    });
                busFactoryConfigurator.ConfigureEndpoints(context);
            });
        });

        // Consul
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            var address = configuration["Consul:Address"] ?? "";
            consulConfig.Address = new Uri(address);
        }));
        services.AddScoped<IConsulServiceDiscovery, ConsulServiceDiscovery>();
        services.AddHostedService<ConsulServiceRegistration>(provider =>
            new ConsulServiceRegistration(
                provider.GetRequiredService<IConsulClient>(),
                provider.GetRequiredService<ILogger<ConsulServiceRegistration>>(),
                configuration["Consul:Service:Host"],
                configuration["Consul:Service:Name"],
                int.Parse(configuration["Consul:Service:Port"])
            ));

        // DI

        // Grpc
        services.AddGrpc(cfg => cfg.EnableDetailedErrors = true);
        services.AddLogging(logging => {
            logging.AddFilter("Grpc", Microsoft.Extensions.Logging.LogLevel.Debug);
        });

        // Logging
        services.AddLoggingSerilog(builder);

        // Diagnostics
        services.AddObservability("Inventory", configuration);

        // Authentication
        services.AddOpenIddict()
            .AddValidation(options =>
            {
                var consulServiceDiscovery = services.BuildServiceProvider().GetRequiredService<IConsulServiceDiscovery>();
                var address = consulServiceDiscovery.GetServiceAddress("user").Result;

                var openIddictConfig = configuration.GetSection("OpenIddict");

                options.SetIssuer($"https://{address}");
                options.AddAudiences(openIddictConfig.GetSection("Audiences").Get<string[]>());

                options.UseIntrospection()
                       .SetClientId(openIddictConfig["Introspection:ClientId"])
                       .SetClientSecret(openIddictConfig["Introspection:ClientSecret"]);

                options
                    .UseSystemNetHttp()
                    .ConfigureHttpClientHandler(handler =>
                    {
                        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    });

                options.UseAspNetCore();
            });

        // CORS
        services.AddCors(options =>
            options.AddPolicy(name: "RentAcmeOrigins", builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials())
        );

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });
        services.AddAuthorization();

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
        // Health
        app.UseHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // CORS
        app.UseCors("RentAcmeOrigins");

        app.UseAuthentication();
        app.UseAuthorization();

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
