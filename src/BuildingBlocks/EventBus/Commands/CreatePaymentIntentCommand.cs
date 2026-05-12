namespace EventBus.Commands;

public class CreatePaymentIntentCommand
{
    public Guid CorrelationId { get; set; }
    public string PaymentMethodId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public decimal TotalPrice { get; set; }
}
