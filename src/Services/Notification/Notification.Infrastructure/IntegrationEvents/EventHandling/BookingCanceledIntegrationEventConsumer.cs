using EventBus.Events;
using GrpcIntegrationHelpers.ClientServices;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Appication.Infrastructure.Services;

namespace Notification.Infrastructure.IntegrationEvents.EventHandling;

public class BookingCanceledIntegrationEventConsumer : IConsumer<BookingCanceledIntegrationEvent>
{
    private readonly ILogger<BookingCanceledIntegrationEventConsumer> _logger;
    private readonly IEmailService _emailService;
    private readonly IUserGrpcClientService _userGrpcClientService;

    public BookingCanceledIntegrationEventConsumer(
        ILogger<BookingCanceledIntegrationEventConsumer> logger,
        IEmailService emailService,
        IUserGrpcClientService userGrpcClientService)
    {
        _logger = logger;
        _emailService = emailService;
        _userGrpcClientService = userGrpcClientService;
    }

    public async Task Consume(ConsumeContext<BookingCanceledIntegrationEvent> context)
    {
        var userId = context.Message.UserId;

        try
        {
            var user = await _userGrpcClientService.GetUserAsync(userId);
            await _emailService.SendAsync(user.Email, "Booking canceled", "Your vehicle reservation has been canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookingCanceledIntegrationEvent");
            throw;
        }

    }
}