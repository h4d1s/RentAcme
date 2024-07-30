using EventBus.Events;
using Inventory.Domain.Common;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.IntegrationEvents.EventHandling;

public class BookingCanceledIntegrationEventConsumer : IConsumer<BookingCanceledIntegrationEvent>
{
    private readonly ILogger<BookingCompletedIntegrationEventConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public BookingCanceledIntegrationEventConsumer(
        ILogger<BookingCompletedIntegrationEventConsumer> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<BookingCanceledIntegrationEvent> context)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(context.Message.BookingId);
        if (booking != null)
        {
            booking.SetAvaliableStatus();
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveEntitiesAsync(CancellationToken.None);
        }
    }
}

