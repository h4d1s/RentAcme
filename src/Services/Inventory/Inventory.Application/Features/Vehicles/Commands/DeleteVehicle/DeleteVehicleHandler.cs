using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Application.Features.Vehicles.Commands.DeleteVehicle;

public class DeleteVehicleHandler : IRequestHandler<DeleteVehicleCommand, Unit>
{
    private readonly IVehicleRepository _vehicleRepository;

    public DeleteVehicleHandler(
        IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
    }

    public async Task<Unit> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);

        if (vehicle is null)
        {
            throw new NotFoundException($"Vehicle with {request.Id} not found.");
        }

        _vehicleRepository.Delete(vehicle);
        await _vehicleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
