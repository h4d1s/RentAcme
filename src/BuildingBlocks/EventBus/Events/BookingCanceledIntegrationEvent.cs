namespace EventBus.Events;

public class BookingCanceledIntegrationEvent
{
    public Guid BookingId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid UserId { get; set; }
}
