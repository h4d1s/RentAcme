using EventBus.Events;
using EventBus.Events.Interfaces;
using GrpcIntegrationHelpers.ClientServices;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Appication.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.IntegrationEvents.EventHandling;

public class BookingCompletedIntegrationEventConsumer : IConsumer<BookingCompletedIntegrationEvent>
{
    private readonly ILogger<BookingCanceledIntegrationEventConsumer> _logger;
    private readonly IEmailService _emailService;
    private readonly IUserGrpcClientService _userGrpcClientService;

    public BookingCompletedIntegrationEventConsumer(
        ILogger<BookingCanceledIntegrationEventConsumer> logger,
        IEmailService emailService,
        IUserGrpcClientService userGrpcClientService)
    {
        _logger = logger;
        _emailService = emailService;
        _userGrpcClientService = userGrpcClientService;
    }

    public async Task Consume(ConsumeContext<BookingCompletedIntegrationEvent> context)
    {
        var userId = context.Message.UserId;

        try
        {
            var user = await _userGrpcClientService.GetUserAsync(userId);
            await _emailService.SendAsync(user.Email, "Booking complete", "Thank you for reservation and payment.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookingCompletedIntegrationEvent");
            throw;
        }
    }
}
