using FluentValidation;
using Inventory.Application.Features.Brands.Commands.CreateBrand;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Models.Commands.CreateModel;

public class CreateModelValidator : AbstractValidator<CreateModelCommand>
{
    public CreateModelValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty();

        RuleFor(p => p.YearOfProduction)
            .InclusiveBetween(1950, DateTime.UtcNow.Year)
            .NotEmpty();

        RuleFor(p => p.NumberOfSeats)
            .NotEmpty();

        RuleFor(p => p.Category)
            .IsInEnum();
    }
}
