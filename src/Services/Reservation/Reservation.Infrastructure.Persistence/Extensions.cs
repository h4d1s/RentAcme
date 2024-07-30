using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using Reservation.Domain.AggregatesModel.BookingAggregate;
using Reservation.Domain.Common;
using Reservation.Infrastructure.Persistence.Data;
using Reservation.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Infrastructure.Persistence;

public static class Extensions
{
    public static IServiceCollection AddInfrastructurePersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Entity Framework
        services.AddDbContext<ReservationContext>(options => options.UseSqlServer(
            configuration.GetConnectionString("ReservationDbContext") ??
                throw new InvalidOperationException("Connection string 'ReservationDbContext' not found.")
            )
        );
        services.AddMigration<ReservationContext, ReservationContextSeed>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
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
