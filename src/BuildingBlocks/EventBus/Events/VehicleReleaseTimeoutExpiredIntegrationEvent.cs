namespace EventBus.Events;

public class VehicleReleaseTimeoutExpiredIntegrationEvent
{
    public Guid BookingId { get; set; }
}
