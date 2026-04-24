using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using User.Application.Infrastructure.Persistence;
using User.Domain.AggregatesModel.ApplicationUserAggregate;
using User.Domain.Common;
using User.Infrastructure.Persistence.Data;
using User.Infrastructure.Persistence.Repositories;

namespace User.Infrastructure.Persistence;

public static class Extensions
{
    public static IServiceCollection AddInfrastructurePersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Entity Framework
        services.AddDbContext<ApplicationUserDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("UserDbContext") ??
                    throw new InvalidOperationException("Connection string 'UserDbContext' not found.")
                );
        });

        // Seed DB
        var serviceProvider = services.BuildServiceProvider();
        var env = serviceProvider.GetRequiredService<IHostEnvironment>();

        if (env.IsDevelopment())
        {
            services.AddMigration<ApplicationUserDbContext, ApplicationUserDbContextSeed>();
        }

        // DI
        services.AddScoped<IUnitOfWork, ApplicationUserDbContext>();
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

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
