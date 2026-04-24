namespace EventBus.Events;

public class BookingCompletedIntegrationEvent
{
    public Guid BookingId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid UserId { get; set; }
    public DateTime PickupDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public decimal Price { get; set; }
}
