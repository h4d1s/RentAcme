using EventBus.Events.Interfaces;

namespace EventBus.Events;

public class PaymentFailedIntegrationEvent : IPaymentFailedIntegrationEvent
{
    public Guid CorrelationId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
