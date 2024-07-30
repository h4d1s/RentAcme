using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Domain.Common;

namespace Inventory.Application.Features.Vehicles.Commands.CreateVehicle;

public class CreateVehicleValidator : AbstractValidator<CreateVehicleCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleValidator(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        //RuleFor(p => p.Gearbox)
        //    .NotEmpty()
        //    .IsInEnum();

        //RuleFor(p => p.YearOfProduction)
        //    .NotEmpty()
        //    .InclusiveBetween(1970, DateTime.Now.Year);

        //RuleFor(p => p.Power)
        //    .NotEmpty();

        //RuleFor(p => p.EngineSize)
        //    .NotEmpty();

        RuleFor(p => p.RentalPricePerDay)
            .NotEmpty()
            .GreaterThanOrEqualTo(5.00m);

        RuleFor(p => p.VariantId)
            .NotEmpty()
            .MustAsync(VariantIdMustExist);
    }

    private async Task<bool> VariantIdMustExist(Guid id, CancellationToken arg2)
    {
        var model = await _unitOfWork.ModelRepository.GetByIdAsync(id);
        return model == null;
    }
}
