using EventBus.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Infrastructure.Hubs;

namespace Notification.Infrastructure.IntegrationEvents.EventHandling;

public class PaymentIntentCreatedIntegrationEventConsumer : IConsumer<PaymentIntentCreatedIntegrationEvent>
{
    private readonly IHubContext<ReservationHub, IReservationClient> _hubContext;

    public PaymentIntentCreatedIntegrationEventConsumer(
        IHubContext<ReservationHub, IReservationClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<PaymentIntentCreatedIntegrationEvent> context)
    {
        var bookingId = context.Message.BookingId;
        var customerId = context.Message.CustomerId;
        var clientSecret = context.Message.ClientSecret;
        var publishableKeySecret = context.Message.PublishableKeySecret;

        await _hubContext.Clients
            .Group($"user:{context.Message.UserId}")
            .OnPaymentIntentCreated(new
            {
                bookingId,
                customerId,
                clientSecret,
                publishableKeySecret
            });
    }
}
