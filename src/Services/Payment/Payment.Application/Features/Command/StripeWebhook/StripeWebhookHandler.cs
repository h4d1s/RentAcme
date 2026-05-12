using EventBus.Events;
using EventBus.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Payment.Application.Exceptions;
using Stripe;

namespace Payment.Application.Features.Command.StripeWebhook;

public class StripeWebhookHandler : IRequestHandler<StripeWebhookCommand, Unit>
{
    private readonly IConfiguration _configuration;
    private readonly IIntegrationEventService _integrationEventService;

    public StripeWebhookHandler(
        IConfiguration configuration,
        IIntegrationEventService integrationEventService)
    {
        _configuration = configuration;
        _integrationEventService = integrationEventService;
    }

    public Task<Unit> Handle(StripeWebhookCommand request, CancellationToken cancellationToken)
    {
        var webhookSecret = _configuration["STRIPE_WEBHOOK_SECRET"] ?? throw new ArgumentNullException("STRIPE_WEBHOOK_SECRET");
        var stripeEvent = EventUtility.ConstructEvent(request.Json, request.StripeSignature, webhookSecret);
        var bookingId = string.Empty;
        var paymentIntent = new PaymentIntent();

        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                if (!paymentIntent.Metadata.TryGetValue("bookingId", out bookingId))
                {
                    throw new BadRequestException($"Booking ID {bookingId} not found in metadata.");
                }

                var paymentCompletedIntegrationEvent = new PaymentCompletedIntegrationEvent
                {
                    BookingId = Guid.Parse(bookingId ?? throw new ArgumentNullException("bookingId")),
                };
                _integrationEventService.PublishAsync(paymentCompletedIntegrationEvent);
                break;

            case "payment_intent.payment_failed":
                paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                if (!paymentIntent.Metadata.TryGetValue("bookingId", out bookingId))
                {
                    throw new BadRequestException($"Booking ID {bookingId} not found in metadata.");
                }
                var paymentFailedIntegrationEvent = new PaymentFailedIntegrationEvent
                {
                    BookingId = Guid.Parse(bookingId ?? throw new ArgumentNullException("bookingId")),
                };
                _integrationEventService.PublishAsync(paymentFailedIntegrationEvent);
                break;
        }

        return Task.FromResult(Unit.Value);
    }
}