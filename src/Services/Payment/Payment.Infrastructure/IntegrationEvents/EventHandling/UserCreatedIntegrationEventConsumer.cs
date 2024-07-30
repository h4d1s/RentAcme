using EventBus.Commands;
using EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Application.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.IntegrationEvents.EventHandling;

public class UserCreatedIntegrationEventConsumer : IConsumer<UserCreatedIntegrationEvent>
{
    private readonly ILogger<CompletePaymentCommandIntegrationEventConsumer> _logger;
    private readonly IPaymentGateway _paymentGateway;

    public UserCreatedIntegrationEventConsumer(
        ILogger<CompletePaymentCommandIntegrationEventConsumer> logger,
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
