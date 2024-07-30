using GrpcIntegrationHelpers.ClientServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcIntegrationHelpers;

public static class GrpcIntegrationHelpersExtensions
{
    public static IServiceCollection AddGrpcIntegrationHelpers(
        this IServiceCollection services)
    {
        // DI
        services.AddScoped<IInventoryGrpcClientService, InventoryGrpcClientService>();
        services.AddScoped<IUserGrpcClientService, UserGrpcClientService>();
        services.AddScoped<INotificationGrpcClientService, NotificationGrpcClientService>();

        return services;
    }
}
