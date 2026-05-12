namespace EventBus.Events;

public class PaymentIntentCreatedIntegrationEvent
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string CustomerId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string PublishableKeySecret { get; set; } = string.Empty;
}
