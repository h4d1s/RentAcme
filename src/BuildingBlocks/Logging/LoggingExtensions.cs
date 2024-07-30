using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Logging;

public static class LoggingExtensions
{
    public static IServiceCollection AddLoggingSerilog(
        this IServiceCollection services,
        IHostBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .CreateLogger();

        builder.UseSerilog(Log.Logger);

        return services;
    }
}
