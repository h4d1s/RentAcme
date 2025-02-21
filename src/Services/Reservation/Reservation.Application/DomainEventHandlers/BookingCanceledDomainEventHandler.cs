﻿using EventBus.Events;
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

public class BookingCanceledDomainEventHandler : INotificationHandler<BookingCanceledDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;
    private readonly ILogger<BookingCanceledDomainEventHandler> _logger;

    public BookingCanceledDomainEventHandler(
        IIntegrationEventService integrationEventService,
        ILogger<BookingCanceledDomainEventHandler> logger)
    {
        _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(BookingCanceledDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Booking with Id: {BookingId} has been successfully updated to status canceled.", notification.Booking.Id);

        var bookingCanceledEvent = new BookingCanceledIntegrationEvent
        {
            VehicleId = notification.Booking.VehicleId
        };
        await _integrationEventService.PublishAsync(bookingCanceledEvent);
    }
}
