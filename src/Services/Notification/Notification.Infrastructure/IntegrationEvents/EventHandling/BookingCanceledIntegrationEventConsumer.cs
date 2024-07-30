using EventBus.Events;
using MassTransit;
using Notification.Appication.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.IntegrationEvents.EventHandling;

public class BookingCanceledIntegrationEventConsumer : IConsumer<BookingCompletedIntegrationEvent>
{
    private readonly IEmailService _emailService;

    public BookingCanceledIntegrationEventConsumer(
        IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<BookingCompletedIntegrationEvent> context)
    {
        //var customerId = context.Message;
        await _emailService.SendAsync("", "Booking canceled", "Your vehicle reservation has been canceled.");
    }
}