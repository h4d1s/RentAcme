namespace Reservation.Application.Features.Bookings.Commands.ReserveBooking;

public class ReserveBookingCommand : IRequest<Guid>
{
    public Guid VehicleId { get; set; }
    public Guid UserId { get; set; }
    public DateTime PickupDate { get; set; }
    public DateTime ReturnDate { get; set; }
}
