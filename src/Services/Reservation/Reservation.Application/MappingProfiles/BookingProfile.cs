using AutoMapper;
using Reservation.Application.Features.Bookings.Commands.ReserveBooking;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.MappingProfiles;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<Booking, ReserveBookingCommand>().ReverseMap();
    }
}
