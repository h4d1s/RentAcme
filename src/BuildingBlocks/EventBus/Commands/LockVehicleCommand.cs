namespace EventBus.Commands;

public class LockVehicleCommand
{
    public Guid BookingId { get; set; } = Guid.Empty;
    public Guid VehicleId { get; set; } = Guid.Empty;
}
