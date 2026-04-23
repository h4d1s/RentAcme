namespace Reservation.Application.Features.Bookings.Commands.CompleteBooking;

public class CompleteBookingCommand : IRequest<Unit>
{
    public Guid BookingId { get; set; }
}
