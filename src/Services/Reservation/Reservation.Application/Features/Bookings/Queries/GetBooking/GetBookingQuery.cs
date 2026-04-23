using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.Features.Bookings.Queries.GetBooking;

public record GetBookingQuery : IRequest<Booking>
{
    public Guid Id { get; set; }
}
