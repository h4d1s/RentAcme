using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Infrastructure.Persistence.Data;
using Reservation.Infrastructure.Persistence.Repositories;

namespace Reservation.Infrastructure.Persistence;

public static class Extensions
{
    public static IServiceCollection AddInfrastructurePersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Extensions).Assembly));

        // Entity Framework
        services.AddDbContext<ReservationDbContext>(options => options.UseSqlServer(
            configuration.GetConnectionString("ReservationDbContext") ??
                throw new InvalidOperationException("Connection string 'ReservationDbContext' not found.")
            )
        );

        // Seed DB
        var serviceProvider = services.BuildServiceProvider();
        var env = serviceProvider.GetRequiredService<IHostEnvironment>();

        if (env.IsDevelopment())
        {
            services.AddMigration<ReservationDbContext, ReservationDbContextSeed>();
        }

        services.AddScoped<IBookingRepository, BookingRepository>();

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
