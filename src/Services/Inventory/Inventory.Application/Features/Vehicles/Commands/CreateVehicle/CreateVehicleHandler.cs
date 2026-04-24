using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Features.Vehicles.Commands.CreateVehicle;

public class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, Guid>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IValidator<CreateVehicleCommand> _validator;
    private readonly ILogger<CreateBrandHandler> _logger;

    public CreateVehicleHandler(
        IVehicleRepository vehicleRepository,
        IValidator<CreateVehicleCommand> validator,
        ILogger<CreateBrandHandler> logger)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("", validationResult);
        }

        var vehicle = new Vehicle(request.RentalPricePerDay, request.RegistrationPlates, request.VariantId);

        _logger.LogInformation("Creating vehicle - Vehicle: {@vehicle}", vehicle);

        var id = await _vehicleRepository.AddAsync(vehicle);
        await _vehicleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return id;
    }


}
