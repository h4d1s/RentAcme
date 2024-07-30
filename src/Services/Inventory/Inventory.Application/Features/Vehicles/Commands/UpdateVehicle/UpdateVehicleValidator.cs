using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Domain.Common;

namespace Inventory.Application.Features.Vehicles.Commands.UpdateVehicle;

public class UpdateVehicleValidator : AbstractValidator<UpdateVehicleCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleValidator(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(p => p.RentalPricePerDay)
            .NotEmpty()
            .GreaterThanOrEqualTo(5.00m);

        RuleFor(p => p.VariantId)
            .NotEmpty()
            .MustAsync(VariantIdMustExist);
    }

    private async Task<bool> VariantIdMustExist(Guid id, CancellationToken arg2)
    {
        var model = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
        return model != null;
    }
}
