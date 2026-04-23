using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using MediatR;

namespace Inventory.Application.Features.Vehicles.Commands.UpdateVehicle;

public class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, Unit>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IValidator<UpdateVehicleCommand> _validator;

    public UpdateVehicleHandler(
        IVehicleRepository vehicleRepository,
        IValidator<UpdateVehicleCommand> validator)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Unit> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid Vehicle", validationResult);
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);

        if (vehicle is null)
        {
            throw new NotFoundException($"Vehicle with {request.Id} not found.");
        }

        vehicle.UpdateRentalPrice(request.RentalPricePerDay);
        vehicle.UpdateVariantId(request.VariantId);
        vehicle.UpdateRegistrationPlates(request.RegistrationPlates);

        _vehicleRepository.Update(vehicle);
        await _vehicleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
