using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reservation.Application.Features.Bookings.Commands.CancelBooking;
using Reservation.Application.Features.Bookings.Commands.CompleteBooking;
using Reservation.Application.Features.Bookings.Commands.ReserveBooking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // DI
        services.AddScoped<IValidator<CancelBookingCommand>, CancelBookingValidator>();
        services.AddScoped<IValidator<CompleteBookingCommand>, CompleteBookingValidator>();
        services.AddScoped<IValidator<ReserveBookingCommand>, ReserveBookingValidator>();

        return services;
    }
}
