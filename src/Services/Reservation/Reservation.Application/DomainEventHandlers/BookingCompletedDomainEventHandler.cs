using EventBus.Events;
using EventBus.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Reservation.Domain.AggregatesModel.BookingAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.DomainEventHandlers;

public class BookingCompletedDomainEventHandler : INotificationHandler<BookingCompletedDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;
    private readonly ILogger<BookingCanceledDomainEventHandler> _logger;

    public BookingCompletedDomainEventHandler(
        IIntegrationEventService integrationEventService,
        ILogger<BookingCanceledDomainEventHandler> logger)
    {
        _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(BookingCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Booking with Id: {BookingId} has been successfully updated to status completed.", notification.Booking.Id);

        var bookingCompletedEvent = new BookingCompletedIntegrationEvent
        {
            BookingId = notification.Booking.Id,
        };
        await _integrationEventService.PublishAsync(bookingCompletedEvent);
    }
}
