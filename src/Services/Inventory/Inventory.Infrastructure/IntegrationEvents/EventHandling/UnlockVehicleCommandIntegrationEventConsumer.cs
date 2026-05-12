using EventBus.Commands;
using EventBus.Events;
using EventBus.Services;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.IntegrationEvents.EventHandling;

public class UnlockVehicleCommandIntegrationEventConsumer : IConsumer<UnlockVehicleCommand>
{
    public IVehicleRepository _vehicleRepository;
    public ILogger<UnlockVehicleCommandIntegrationEventConsumer> _logger;

    public UnlockVehicleCommandIntegrationEventConsumer(
        IVehicleRepository vehicleRepository,
        ILogger<UnlockVehicleCommandIntegrationEventConsumer> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UnlockVehicleCommand> context)
    {
        var vehicleId = context.Message.VehicleId;

        try
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            if (vehicle is null)
            {
                SendVehicleUnavaliableIntegrationEvent(context, context.Message.BookingId);
                return;
            }

            vehicle.UpdateIsLocked(false);
            _vehicleRepository.Update(vehicle);
            await _vehicleRepository.UnitOfWork.SaveChangesAsync();

            var vehicleUnlockedIntegrationEvent = new VehicleUnlockedIntegrationEvent
            {
                BookingId = context.Message.BookingId
            };
            await context.Publish(vehicleUnlockedIntegrationEvent);
        }
        catch (Exception)
        {
            SendVehicleUnavaliableIntegrationEvent(context, context.Message.BookingId);
        }
    }

    private async void SendVehicleUnavaliableIntegrationEvent(ConsumeContext<UnlockVehicleCommand> context, Guid bookingId)
    {
        _logger.LogWarning($"Vehicle not avaliable for booking: {bookingId}");

        var vehicleUnavaliableIntegrationEvent = new VehicleUnavailableIntegrationEvent
        {
            BookingId = bookingId
        };
        await context.Publish(vehicleUnavaliableIntegrationEvent);
    }
}
