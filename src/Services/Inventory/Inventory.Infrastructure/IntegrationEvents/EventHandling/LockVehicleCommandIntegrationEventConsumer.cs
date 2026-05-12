using EventBus.Commands;
using EventBus.Events;
using EventBus.Services;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using static Identity.Models.Permissions;

namespace Inventory.Infrastructure.IntegrationEvents.EventHandling;

public class LockVehicleCommandIntegrationEventConsumer : IConsumer<LockVehicleCommand>
{
    public IVehicleRepository _vehicleRepository;
    public ILogger<LockVehicleCommandIntegrationEventConsumer> _logger;

    public LockVehicleCommandIntegrationEventConsumer(
        IVehicleRepository vehicleRepository,
        ILogger<LockVehicleCommandIntegrationEventConsumer> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LockVehicleCommand> context)
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

            if (vehicle.IsLocked)
            {
                SendVehicleUnavaliableIntegrationEvent(context, context.Message.BookingId);
                return;
            }

            vehicle.UpdateIsLocked(true);
            _vehicleRepository.Update(vehicle);
            await _vehicleRepository.UnitOfWork.SaveChangesAsync();

            var vehicleLockedIntegrationEvent = new VehicleLockedIntegrationEvent
            {
                BookingId = context.Message.BookingId
            };
            await context.Publish(vehicleLockedIntegrationEvent);
        }
        catch (Exception)
        {
            SendVehicleUnavaliableIntegrationEvent(context, context.Message.BookingId);
        }
    }

    private async void SendVehicleUnavaliableIntegrationEvent(ConsumeContext<LockVehicleCommand> context, Guid bookingId)
    {
        _logger.LogWarning($"Vehicle not avaliable for booking: {bookingId}");

        var vehicleUnavaliableIntegrationEvent = new VehicleUnavailableIntegrationEvent
        {
            BookingId = bookingId
        };
        await context.Publish(vehicleUnavaliableIntegrationEvent);
    }
}
