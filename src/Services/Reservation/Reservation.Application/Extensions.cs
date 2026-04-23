using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reservation.Application.Features.Bookings.Commands.CancelBooking;
using Reservation.Application.Features.Bookings.Commands.CompleteBooking;
using Reservation.Application.Features.Bookings.Commands.ReserveBooking;
using System.Reflection;

namespace Reservation.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // DI
        services.AddScoped<IValidator<CancelBookingCommand>, CancelBookingValidator>();
        services.AddScoped<IValidator<CompleteBookingCommand>, CompleteBookingValidator>();
        services.AddScoped<IValidator<ReserveBookingCommand>, ReserveBookingValidator>();

        return services;
    }
}
