using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Brands.Commands.UpdateBrand;

public class UpdateBrandHandler : IRequestHandler<UpdateBrandCommand, Unit>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IValidator<UpdateBrandCommand> _validator;

    public UpdateBrandHandler(
        IBrandRepository brandRepository,
        IValidator<UpdateBrandCommand> validator)
    {
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Unit> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid Brand", validationResult);
        }

        var brand = await _brandRepository.GetByIdAsync(request.Id);

        if (brand is null)
        {
            throw new NotFoundException($"Brand with {request.Id} not found.");
        }

        brand.UpdateName(request.Name);

        _brandRepository.Update(brand);
        await _brandRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
