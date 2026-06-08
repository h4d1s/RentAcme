using Reservation.Domain.AggregatesModel.BookingAggregate;

namespace Reservation.Application.Features.Bookings.Dtos;

public class BookingCacheDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Guid UserId { get; set; }
    public DateTime PickupDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime? BookingDate { get; set; }
    public decimal Price { get; set; }
}
