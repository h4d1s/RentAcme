using EventBus.Events;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.IntegrationEvents.EventHandling;

public class BookingCompletedIntegrationEventConsumer : IConsumer<BookingCompletedIntegrationEvent>
{
    private readonly ILogger<BookingCompletedIntegrationEventConsumer> _logger;
    private readonly IVehicleRepository _vehicleRepository;

    public BookingCompletedIntegrationEventConsumer(
        ILogger<BookingCompletedIntegrationEventConsumer> logger,
        IVehicleRepository vehicleRepository)
    {
        _logger = logger;
        _vehicleRepository = vehicleRepository;
    }

    public async Task Consume(ConsumeContext<BookingCompletedIntegrationEvent> context)
    {
        try
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(context.Message.VehicleId);
            if (vehicle is not null)
            {
                vehicle.UpdateIsLocked(false);
                _vehicleRepository.Update(vehicle);
                await _vehicleRepository.UnitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookingCompletedIntegrationEvent");
            throw;
        }
    }
}
