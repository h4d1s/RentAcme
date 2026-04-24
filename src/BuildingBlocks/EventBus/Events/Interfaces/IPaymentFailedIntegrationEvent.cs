using MassTransit;

namespace EventBus.Events.Interfaces;

public interface IPaymentFailedIntegrationEvent : CorrelatedBy<Guid>
{
    public string ErrorMessage { get; set; }
    public Guid UserId { get; set; }
}
