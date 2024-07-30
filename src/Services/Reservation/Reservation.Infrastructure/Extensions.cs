using Diagnostics;
using Google.Protobuf.WellKnownTypes;
using Logging;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using OpenIddict.Validation.AspNetCore;
using Microsoft.AspNetCore.Routing;
using Consul;
using EventBus;
using EventBus.Services;
using ConsulIntegrationHelpers.Services;
using ConsulIntegrationHelpers.HostedServices;
using Microsoft.Extensions.Logging;
using GrpcIntegrationHelpers.ClientServices;
using GrpcIntegrationHelpers;

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
                configuration["ConnectionStrings:ReservationDbContext"],
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

        // OpenIdDict
        services.AddOpenIddict()
            .AddValidation(options =>
            {
                var consulServiceDiscovery = services.BuildServiceProvider().GetRequiredService<IConsulServiceDiscovery>();
                try
                {
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
                }
                catch (Exception)
                {
                    
                }
            });

        // CORS
        services.AddCors(options =>
            options.AddPolicy(name: "RentAcmeOrigins", builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials())
        );

        services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });
        services.AddAuthorization();

        // Logging
        services.AddLoggingSerilog(builder);

        // Diagnostics
        services.AddObservability("Reservation", configuration);

        // Event Bus
        services.AddEventBus();

        // GrpcHelpers
        services.AddGrpcIntegrationHelpers();

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

        // CORS
        app.UseCors("RentAcmeOrigins");

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
