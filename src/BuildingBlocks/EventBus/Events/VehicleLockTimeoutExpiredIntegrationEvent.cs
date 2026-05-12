namespace EventBus.Events;

public class VehicleLockTimeoutExpiredIntegrationEvent
{
    public Guid BookingId { get; set; }
}
