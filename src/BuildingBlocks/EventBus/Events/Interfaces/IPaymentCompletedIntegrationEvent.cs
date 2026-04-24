using MassTransit;

namespace EventBus.Events.Interfaces;

public interface IPaymentCompletedIntegrationEvent : CorrelatedBy<Guid>
{
    public string CustomerId { get; set; }
}
