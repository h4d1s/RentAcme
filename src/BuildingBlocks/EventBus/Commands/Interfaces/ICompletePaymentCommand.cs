using MassTransit;

namespace EventBus.Commands.Interfaces;

public interface ICompletePaymentCommand : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public decimal TotalPrice { get; set; }
}
