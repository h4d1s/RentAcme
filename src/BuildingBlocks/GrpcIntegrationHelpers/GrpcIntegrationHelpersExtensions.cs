using GrpcIntegrationHelpers.ClientServices;
using Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GrpcIntegrationHelpers;

public static class GrpcIntegrationHelpersExtensions
{
    public static IServiceCollection AddGrpcIntegrationHelpers(
        this IServiceCollection services,
        IHostBuilder builder,
        IConfiguration configuration)
    {
        // DI
        services.AddHttpContextAccessor();
        services.AddScoped<IInventoryGrpcClientService, InventoryGrpcClientService>();
        services.AddScoped<IUserGrpcClientService, UserGrpcClientService>();

        services.AddIdentityServices(builder, configuration);

        return services;
    }
}
