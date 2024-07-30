using FluentValidation;
using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Application.Features.Brands.Commands.UpdateBrand;
using Inventory.Application.Features.Models.Commands.CreateModel;
using Inventory.Application.Features.Models.Commands.UpdateModel;
using Inventory.Application.Features.Variants.Commands.CreateVariant;
using Inventory.Application.Features.Variants.Commands.UpdateVariant;
using Inventory.Application.Features.Vehicles.Commands.CreateVehicle;
using Inventory.Application.Features.Vehicles.Commands.UpdateVehicle;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // DI
        services.AddScoped<IValidator<CreateVehicleCommand>, CreateVehicleValidator>();
        services.AddScoped<IValidator<UpdateVehicleCommand>, UpdateVehicleValidator>();
        services.AddScoped<IValidator<CreateVariantCommand>, CreateVariantValidator>();
        services.AddScoped<IValidator<UpdateVariantCommand>, UpdateVariantValidator>();
        services.AddScoped<IValidator<CreateModelCommand>, CreateModelValidator>();
        services.AddScoped<IValidator<UpdateModelCommand>, UpdateModelValidator>();
        services.AddScoped<IValidator<CreateBrandCommand>, CreateBrandValidator>();
        services.AddScoped<IValidator<UpdateBrandCommand>, UpdateBrandValidator>();

        return services;
    }

    public static IApplicationBuilder ConfigureApplicationServices(
        this IApplicationBuilder app,
        IHostEnvironment environment,
        IServiceProvider serviceProvider)
    {

        return app;
    }
}
