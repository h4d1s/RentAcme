using Inventory.Application.Infrastructure.Persistence;
using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using Inventory.Infrastructure.Persistence.Data;
using Inventory.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;

namespace Inventory.Infrastructure.Persistence;

public static class Extensions
{
    public static IServiceCollection AddInfrastructurePersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Entity Framework
        services.AddDbContext<InventoryDbContext>(options => options.UseNpgsql(
            configuration.GetConnectionString("InventoryDbContext") ??
                throw new InvalidOperationException("Connection string 'InventoryDbContext' not found.")
            )
        );

        // Seed DB
        var serviceProvider = services.BuildServiceProvider();
        var env = serviceProvider.GetRequiredService<IHostEnvironment>();

        if (env.IsDevelopment())
        {
            services.AddMigration<InventoryDbContext, InventoryDbContextSeed>();
        }

        // DI
        services.AddScoped<IUnitOfWork, InventoryDbContext>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IModelRepository, ModelRepository>();
        services.AddScoped<IVariantRepository, VariantRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();

        return services;
    }

    public static IApplicationBuilder ConfigureInfrastructurePersistenceServices(
        this IApplicationBuilder app,
        IHostEnvironment environment,
        IServiceProvider serviceProvider)
    {
        return app;
    }
}
