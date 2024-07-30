using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Application.Features.Models.Commands.UpdateModel;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventory.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Variants.Commands.UpdateVariant;

public class UpdateVariantHandler : IRequestHandler<UpdateVariantCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateVariantCommand> _validator;

    public UpdateVariantHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateVariantCommand> validator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Unit> Handle(UpdateVariantCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid Variant", validationResult);
        }

        var variant = await _unitOfWork.VariantRepository.GetByIdAsync(request.Id);

        if (variant == null)
        {
            throw new NotFoundException($"Variant with {request.Id} not found.");
        }

        _mapper.Map(request, variant);

        _unitOfWork.VariantRepository.Update(variant);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
