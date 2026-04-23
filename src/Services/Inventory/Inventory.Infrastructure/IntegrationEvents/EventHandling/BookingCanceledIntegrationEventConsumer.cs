using EventBus.Events;
using Inventory.Domain.AggregatesModel.BookingAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.IntegrationEvents.EventHandling;

public class BookingCanceledIntegrationEventConsumer : IConsumer<BookingCanceledIntegrationEvent>
{
    private readonly ILogger<BookingCompletedIntegrationEventConsumer> _logger;
    private readonly IBookingRepository _bookingRepository;

    public BookingCanceledIntegrationEventConsumer(
        ILogger<BookingCompletedIntegrationEventConsumer> logger,
        IBookingRepository bookingRepository)
    {
        _logger = logger;
        _bookingRepository = bookingRepository;
    }

    public async Task Consume(ConsumeContext<BookingCanceledIntegrationEvent> context)
    {
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(context.Message.BookingId);
            if (booking is not null)
            {
                booking.SetAvaliableStatus();
                _bookingRepository.Update(booking);
                await _bookingRepository.UnitOfWork.SaveEntitiesAsync(CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookingCanceledIntegrationEvent");
            throw;
        }
    }
}

