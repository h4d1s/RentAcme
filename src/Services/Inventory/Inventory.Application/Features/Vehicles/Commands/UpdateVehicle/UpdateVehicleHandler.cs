using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using MediatR;

namespace Inventory.Application.Features.Vehicles.Commands.UpdateVehicle;

public class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateVehicleCommand> _validator;

    public UpdateVehicleHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateVehicleCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Unit> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid Vehicle", validationResult);
        }

        var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(request.Id);

        if (vehicle == null)
        {
            throw new NotFoundException($"Vehicle with {request.Id} not found.");
        }

        _mapper.Map(request, vehicle);

        _unitOfWork.VehicleRepository.Update(vehicle);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
