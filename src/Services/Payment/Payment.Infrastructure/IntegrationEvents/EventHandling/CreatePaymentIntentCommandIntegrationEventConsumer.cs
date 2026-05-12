using EventBus.Commands;
using EventBus.Events;
using GrpcIntegrationHelpers.ClientServices;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Payment.Application.Infrastructure.Services;

namespace Payment.Infrastructure.IntegrationEvents.EventHandling;

public class CreatePaymentIntentCommandIntegrationEventConsumer : IConsumer<CreatePaymentIntentCommand>
{
    private readonly ILogger<CreatePaymentIntentCommandIntegrationEventConsumer> _logger;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUserGrpcClientService _userGrpcClientService;
    private readonly IConfiguration _configuration;

    public CreatePaymentIntentCommandIntegrationEventConsumer(
        ILogger<CreatePaymentIntentCommandIntegrationEventConsumer> logger,
        IPaymentGateway paymentGateway,
        IUserGrpcClientService userGrpcClientService,
        IConfiguration configuration)
    {
        _logger = logger;
        _paymentGateway = paymentGateway;
        _userGrpcClientService = userGrpcClientService;
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<CreatePaymentIntentCommand> context)
    {
        var userId = context.Message.UserId;
        var amount = context.Message.TotalPrice;
        var bookingId = context.Message.CorrelationId;
        var paymentMethodId = context.Message.PaymentMethodId;

        var user = await _userGrpcClientService.GetUserAsync(userId);
        var customer = await _paymentGateway.GetCustomerByEmailAsync(user.Email);

        if (customer == null)
        {
            _logger.LogError($"Could not retrieve customer with email: {user.Email}");
            var paymentFailedIntegrationEvent = new PaymentFailedIntegrationEvent
            {
                BookingId = bookingId,
                UserId = userId,
                ErrorMessage = $"Could not retrieve customer with email: {user.Email}"
            };
            await context.Publish(paymentFailedIntegrationEvent);
            return;
        }

        try
        {
            var clientSecret = await _paymentGateway.CreateIntentAsync(bookingId, customer.Id, amount);
            var publishableKeySecret = _configuration["STRIPE_PUBLISHABLE_KEY"] ?? throw new ArgumentNullException("STRIPE_PUBLISHABLE_KEY");

            var paymentIntentCreatedIntegrationEvent = new PaymentIntentCreatedIntegrationEvent
            {
                BookingId = bookingId,
                UserId = userId,
                CustomerId = customer.Id,
                ClientSecret = clientSecret,
                PublishableKeySecret = publishableKeySecret
            };
            await context.Publish(paymentIntentCreatedIntegrationEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not charge customer with email: {user.Email}, {ex}");
            var paymentFailedIntegrationEvent = new PaymentFailedIntegrationEvent
            {
                BookingId = bookingId,
                UserId = userId,
                ErrorMessage = $"{ex.Message}"
            };
            await context.Publish(paymentFailedIntegrationEvent);
        }
    }
}
