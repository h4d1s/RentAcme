namespace EventBus.Events;

public class PaymentFailedIntegrationEvent
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
