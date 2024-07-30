using EventBus.Events;
using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.Common;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.IntegrationEvents.EventHandling;

public class BookingCompletedIntegrationEventConsumer : IConsumer<BookingCompletedIntegrationEvent>
{
    private readonly ILogger<BookingCompletedIntegrationEventConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public BookingCompletedIntegrationEventConsumer(
        ILogger<BookingCompletedIntegrationEventConsumer> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<BookingCompletedIntegrationEvent> context)
    {
        var booking = new Booking(BookingStatus.Reserved, context.Message.VehicleId);
        booking.SetPickupDate(context.Message.PickupDate);
        booking.SetReturnDate(context.Message.ReturnDate);
        await _unitOfWork.BookingRepository.AddAsync(booking);
    }
}
