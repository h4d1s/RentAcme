using EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Application.Infrastructure.Services;

namespace Payment.Infrastructure.IntegrationEvents.EventHandling;

public class UserCreatedIntegrationEventConsumer : IConsumer<UserCreatedIntegrationEvent>
{
    private readonly ILogger<CreatePaymentIntentCommandIntegrationEventConsumer> _logger;
    private readonly IPaymentGateway _paymentGateway;

    public UserCreatedIntegrationEventConsumer(
        ILogger<CreatePaymentIntentCommandIntegrationEventConsumer> logger,
        IPaymentGateway paymentGateway)
    {
        _logger = logger;
        _paymentGateway = paymentGateway;
    }

    public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        await _paymentGateway.CreateCustomerAsync(
            context.Message.FirstName,
            context.Message.LastName,
            context.Message.Email);
    }
}
