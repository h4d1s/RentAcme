using EventBus;
using EventBus.Constants;
using EventBus.Events;
using EventBus.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Reservation.Domain.AggregatesModel.BookingAggregate.Events;
using Reservation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservation.Application.DomainEventHandlers;

public class BookingReservedDomainEventHandler : INotificationHandler<BookingReservedDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;
    private readonly ILogger<BookingCanceledDomainEventHandler> _logger;

    public BookingReservedDomainEventHandler(
        ILogger<BookingCanceledDomainEventHandler> logger,
        IIntegrationEventService integrationEventService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
    }

    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Booking with Id: {BookingId} has been successfully reserved.", notification.Booking.Id);

        var bookingReservedEvent = new BookingReservedIntegrationEvent
        {
            BookingId = notification.Booking.Id,
            UserId = notification.Booking.UserId,
            VehicleId = notification.Booking.VehicleId,
            PickupDate = notification.Booking.PickupDate,
            ReturnDate = notification.Booking.ReturnDate,
            Price = notification.Booking.Price,
        };
        await _integrationEventService.PublishAsync(bookingReservedEvent);
    }
}
