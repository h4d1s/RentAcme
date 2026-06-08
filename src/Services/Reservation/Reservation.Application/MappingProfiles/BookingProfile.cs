using AutoMapper;
using Reservation.Application.Features.Bookings.Dtos;
using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.MappingProfiles;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<Booking, BookingCacheDto>();
        CreateMap<BookingCacheDto, Booking>()
            .ConstructUsing(src => new Booking(
                src.VehicleId,
                src.UserId,
                src.PickupDate,
                src.ReturnDate
            ));
    }
}
