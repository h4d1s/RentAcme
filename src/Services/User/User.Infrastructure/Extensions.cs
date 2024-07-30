using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using User.Domain.AggregatesModel.ApplicationUser;
using Microsoft.AspNetCore.Identity;
using User.Infrastructure.Persistence.Data;
using User.Infrastructure.Services;
using User.Application.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using User.Infrastructure.Grpc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Logging;
using Diagnostics;
using Microsoft.AspNetCore.Routing;
using User.Infrastructure.HostedServices;
using static OpenIddict.Abstractions.OpenIddictConstants;
using EventBus;
using EventBus.Services;
using ConsulIntegrationHelpers.HostedServices;
using ConsulIntegrationHelpers.Services;
using Consul;

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
            .AddSqlServer(
                configuration["ConnectionStrings:UserDbContext"],
                name: "UserDB-check",
                tags: new string[] { "userdb" });

        // Identity
        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<UserContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserService, UserService>();

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

        // OpenIddict
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<UserContext>();
            })
            .AddServer(options =>
            {
                options
                       .RequireProofKeyForCodeExchange()
                       .AllowRefreshTokenFlow()
                       .AllowPasswordFlow();

                options
                       .SetTokenEndpointUris("connect/token")
                       .SetIntrospectionEndpointUris("connect/introspect")
                       .SetUserinfoEndpointUris("connect/userinfo")
                       .SetLogoutEndpointUris("connect/logout");

                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableLogoutEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableUserinfoEndpointPassthrough()
                       .EnableStatusCodePagesIntegration();

                options.AcceptAnonymousClients();

                var httpsPort = Environment.GetEnvironmentVariable("SERVICE_HTTPS_PORTS");

                options.Configure(options =>
                {
                    options.TokenValidationParameters.ValidIssuers = new List<string>
                    {
                        $"https://localhost:{httpsPort}/",
                    };
                });
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        services.AddHostedService<OpenIdDictServiceRegistration>();

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

        // Grpc
        services.AddGrpc(cfg => cfg.EnableDetailedErrors = true);
        services.AddLogging(logging => {
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
        services.AddAuthorization();

        // Serilog
        services.AddLoggingSerilog(builder);

        // Diagnostics
        services.AddObservability("User", configuration);

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

        // Authentication
        app.UseAuthentication();
        app.UseAuthorization();

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
