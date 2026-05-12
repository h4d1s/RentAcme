namespace EventBus.Commands;

public class UnlockVehicleCommand
{
    public Guid BookingId { get; set; } = Guid.Empty;
    public Guid VehicleId { get; set; } = Guid.Empty;
}
