namespace EventBus.Events;

public class PaymentCompletedIntegrationEvent
{
    public Guid BookingId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}
