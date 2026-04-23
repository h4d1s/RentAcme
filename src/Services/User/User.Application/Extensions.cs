using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using User.Application.Features.Users.Commands.DeleteUser;
using User.Application.Features.Users.Commands.UpdateUser;
using User.Application.Features.Users.Queries.GetUser;

namespace User.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IHostEnvironment environment)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

        // DI
        services.AddScoped<IValidator<DeleteUserCommand>, DeleteUserValidator>();
        services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserValidator>();

        return services;
    }
}
