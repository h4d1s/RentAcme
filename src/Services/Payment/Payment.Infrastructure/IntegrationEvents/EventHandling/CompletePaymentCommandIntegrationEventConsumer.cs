using EventBus.Commands;
using EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Application.Infrastructure.Services;
using Payment.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrpcIntegrationHelpers.ClientServices;
using EventBus.Events.Interfaces;

namespace Payment.Infrastructure.IntegrationEvents.EventHandling;

public class CompletePaymentCommandIntegrationEventConsumer : IConsumer<CompletePaymentCommand>
{
    private readonly ILogger<CompletePaymentCommandIntegrationEventConsumer> _logger;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUserGrpcClientService _userService;

    public CompletePaymentCommandIntegrationEventConsumer(
        ILogger<CompletePaymentCommandIntegrationEventConsumer> logger,
        IPaymentGateway paymentGateway,
        IUserGrpcClientService userService)
    {
        _logger = logger;
        _paymentGateway = paymentGateway;
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<CompletePaymentCommand> context)
    {
        var userId = context.Message.UserId;
        var amount = context.Message.TotalPrice;

        var user = await _userService.GetUserAsync(userId);
        var customer = await _paymentGateway.GetCustomerByEmailAsync(user.Email);

        if (customer == null)
        {
            _logger.LogError($"Could not retrieve customer with email: {user.Email}");
            var paymentFailedIntegrationEvent = new PaymentFailedIntegrationEvent
            {
                CorrelationId = context.Message.CorrelationId,
                UserId = userId,
                ErrorMessage = $"Could not retrieve customer with email: {user.Email}"
            };
            await context.Publish(paymentFailedIntegrationEvent);
        }
        else
        {
            try
            {
                await _paymentGateway.ChargeAsync(customer.Id, amount);
                var paymentCompletedIntegrationEvent = new PaymentCompletedIntegrationEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    CustomerId = customer.Id,
                };
                await context.Publish(paymentCompletedIntegrationEvent);
            }
            catch (Exception ex)
            {
                var paymentFailedIntegrationEvent = new PaymentFailedIntegrationEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    UserId = userId,
                    ErrorMessage = $"{ex.Message}"
                };
                await context.Publish(paymentFailedIntegrationEvent);
            }
        }
    }
}
