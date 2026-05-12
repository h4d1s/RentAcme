namespace EventBus.Events;

public class PaymentTimeoutExpiredIntegrationEvent
{
    public Guid BookingId { get; set; }
}
