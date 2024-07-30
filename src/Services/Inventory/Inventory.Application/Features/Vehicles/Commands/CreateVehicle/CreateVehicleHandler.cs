using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Features.Vehicles.Commands.CreateVehicle;

public class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, Guid>
{
    private IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateVehicleCommand> _validator;
    private readonly ILogger<CreateBrandHandler> _logger;

    public CreateVehicleHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateVehicleCommand> validator,
        ILogger<CreateBrandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        var vehicle = _mapper.Map<Vehicle>(request);

        _logger.LogInformation("Creating vehicle - Vehicle: {@vehicle}", vehicle);

        var id = await _unitOfWork.VehicleRepository.AddAsync(vehicle);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return id;
    }
}
