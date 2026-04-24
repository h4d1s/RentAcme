using EventBus.Events.Interfaces;

namespace EventBus.Events;

public class PaymentCompletedIntegrationEvent : IPaymentCompletedIntegrationEvent
{
    public Guid CorrelationId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}
