namespace EventBus.Events;

public class VehicleLockedIntegrationEvent
{
    public Guid BookingId { get; set; } = Guid.Empty;
    public Guid VehicleId { get; set; } = Guid.Empty;
}
