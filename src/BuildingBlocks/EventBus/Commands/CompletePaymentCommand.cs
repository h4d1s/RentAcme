using EventBus.Commands.Interfaces;

namespace EventBus.Commands;

public class CompletePaymentCommand : ICompletePaymentCommand
{
    public Guid CorrelationId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalPrice { get; set; }
}
