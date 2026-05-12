using MediatR;

namespace Payment.Application.Features.Command.StripeWebhook;

public class StripeWebhookCommand : IRequest<Unit>
{
    public string Json = string.Empty;
    public string StripeSignature = string.Empty;
}
