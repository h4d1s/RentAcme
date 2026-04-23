namespace Reservation.Application.Features.Bookings.Commands.CancelBooking;

public class CancelBookingCommand : IRequest<Unit>
{
    public Guid BookingId { get; set; }
}
