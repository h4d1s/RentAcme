using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using User.Application.Features.Users.Commands.SignIn;
using User.Application.Features.Users.Commands.SignUp;
using User.Application.Features.Users.Commands.UserInfo;

namespace User.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IHostEnvironment environment)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IValidator<SignInCommand>, SignInCommandValidator>();
        services.AddScoped<IValidator<SignUpCommand>, SignUpCommandValidator>();
        services.AddScoped<IValidator<UserInfoCommand>, UserInfoCommandValidator>();

        return services;
    }
}
