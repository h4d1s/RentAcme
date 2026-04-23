using FluentValidation;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Models.Commands.UpdateModel;

public class UpdateModelValidator : AbstractValidator<UpdateModelCommand>
{
    private readonly IModelRepository _modelRepository;

    public UpdateModelValidator(
        IModelRepository modelRepository)
    {
        _modelRepository = modelRepository;

        RuleFor(p => p.Id)
            .MustAsync(ModelIdMustExist)
            .WithMessage("{PropertyName} does not exist.");

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

    private async Task<bool> ModelIdMustExist(Guid id, CancellationToken arg2)
    {
        var model = await _modelRepository.GetByIdAsync(id);
        return model is not null;
    }
}
