using Inventory.Application.Exceptions;
using Inventory.Domain.Common;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Application.Features.Vehicles.Commands.DeleteVehicle;

public class DeleteVehicleHandler : IRequestHandler<DeleteVehicleCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVehicleHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Unit> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(request.Id);

        if (vehicle == null)
        {
            throw new NotFoundException($"Vehicle with {request.Id} not found.");
        }

        _unitOfWork.VehicleRepository.Delete(vehicle);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
