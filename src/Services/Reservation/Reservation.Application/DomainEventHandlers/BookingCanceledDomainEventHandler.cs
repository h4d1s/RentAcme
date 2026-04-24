using EventBus.Events;
using EventBus.Services;
using Microsoft.Extensions.Logging;
using Reservation.Domain.AggregatesModel.BookingAggregate.Events;

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
            BookingId = notification.Booking.Id,
            VehicleId = notification.Booking.VehicleId,
            UserId = notification.Booking.UserId
        };
        await _integrationEventService.PublishAsync(bookingCanceledEvent);
    }
}
