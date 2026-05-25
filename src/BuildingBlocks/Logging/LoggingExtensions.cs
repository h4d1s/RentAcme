using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Logging;

public static class LoggingExtensions
{
    public static IServiceCollection AddLoggingSerilog(
        this IServiceCollection services,
        string serviceName,
        IHostBuilder builder,
        IConfiguration configuration)
    {
        var endpoint = configuration["OtelCollector:Endpoint"] ?? throw new ArgumentNullException("OtelCollector:Endpoint");

        ConfigureGlobalLogger(serviceName, endpoint);

        builder.UseSerilog(Log.Logger);

        return services;
    }

    public static IServiceCollection AddLoggingSerilog(
    this IServiceCollection services,
    string serviceName,
    IHostApplicationBuilder builder,
    IConfiguration configuration)
    {
        var endpoint = configuration["OtelCollector:Endpoint"] ?? throw new ArgumentNullException("OtelCollector:Endpoint");

        ConfigureGlobalLogger(serviceName, endpoint);

        builder.Services.AddSerilog(Log.Logger);

        return services;
    }

    private static void ConfigureGlobalLogger(string serviceName, string endpoint)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = endpoint;
                options.Protocol = OtlpProtocol.Grpc;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = serviceName,
                    ["service.version"] = "1.0.0"
                };
            })
            .CreateLogger();
    }
}
