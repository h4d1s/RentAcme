using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using User.Application.Features.Users.Commands.DeleteUser;
using User.Application.Features.Users.Commands.UpdateUser;

namespace User.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IHostEnvironment environment)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // DI
        services.AddScoped<IValidator<DeleteUserCommand>, DeleteUserValidator>();
        services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserValidator>();

        return services;
    }
}
