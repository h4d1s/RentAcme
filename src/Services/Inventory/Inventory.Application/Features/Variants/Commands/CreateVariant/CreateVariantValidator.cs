using FluentValidation;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Variants.Commands.CreateVariant;

public class CreateVariantValidator : AbstractValidator<CreateVariantCommand>
{
    public CreateVariantValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty();

        RuleFor(p => p.Gearbox)
            .IsInEnum();

        RuleFor(p => p.FuelType)
            .IsInEnum();

        RuleFor(p => p.Power)
            .GreaterThan(0);

        RuleFor(p => p.EngineSize)
            .GreaterThan(0);
    }
}
