using EventBus.Events;
using EventBus.Events.Interfaces;
using MassTransit;
using Notification.Appication.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.IntegrationEvents.EventHandling;

public class BookingCompletedIntegrationEventConsumer : IConsumer<BookingCompletedIntegrationEvent>
{
    private readonly IEmailService _emailService;

    public BookingCompletedIntegrationEventConsumer(
        IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<BookingCompletedIntegrationEvent> context)
    {
        //var bookingId = context.Message.BookingId;
        await _emailService.SendAsync("", "Booking complete", "Thank you for reservation and payment.");
    }
}
