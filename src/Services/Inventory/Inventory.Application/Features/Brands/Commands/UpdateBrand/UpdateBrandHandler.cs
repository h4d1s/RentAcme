using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateBrandCommand> _validator;

    public UpdateBrandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateBrandCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Unit> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid Brand", validationResult);
        }

        var brand = await _unitOfWork.BrandRepository.GetByIdAsync(request.Id);

        if (brand == null)
        {
            throw new NotFoundException($"Brand with {request.Id} not found.");
        }

        _mapper.Map(request, brand);

        _unitOfWork.BrandRepository.Update(brand);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
