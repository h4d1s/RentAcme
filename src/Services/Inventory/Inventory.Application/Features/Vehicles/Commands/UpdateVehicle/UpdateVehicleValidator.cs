using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Domain.Common;
using Inventory.Domain.AggregatesModel.VehicleAggregate;

namespace Inventory.Application.Features.Vehicles.Commands.UpdateVehicle;

public class UpdateVehicleValidator : AbstractValidator<UpdateVehicleCommand>
{
    private readonly IVehicleRepository _vehicleRepository;

    public UpdateVehicleValidator(
        IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));

        RuleFor(p => p.RentalPricePerDay)
            .NotEmpty()
            .GreaterThanOrEqualTo(5.00m);

        RuleFor(p => p.VariantId)
            .NotEmpty()
            .MustAsync(VariantIdMustExist);
    }

    private async Task<bool> VariantIdMustExist(Guid id, CancellationToken arg2)
    {
        var model = await _vehicleRepository.GetByIdAsync(id);
        return model is not null;
    }
}
