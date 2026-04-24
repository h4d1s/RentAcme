using AutoMapper;
using FluentValidation;
using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;

namespace Inventory.Application.Features.Variants.Commands.UpdateVariant;

public class UpdateVariantHandler : IRequestHandler<UpdateVariantCommand, Unit>
{
    private readonly IVariantRepository _variantRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateVariantCommand> _validator;

    public UpdateVariantHandler(
        IVariantRepository variantRepository,
        IMapper mapper,
        IValidator<UpdateVariantCommand> validator)
    {
        _variantRepository = variantRepository ?? throw new ArgumentNullException(nameof(variantRepository));
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

        var variant = await _variantRepository.GetByIdAsync(request.Id);

        if (variant is null)
        {
            throw new NotFoundException($"Variant with {request.Id} not found.");
        }

        variant.UpdateName(request.Name);
        variant.UpdateGearbox(request.Gearbox);
        variant.UpdateFuelType(request.FuelType);
        variant.UpdatePower(request.Power);
        variant.UpdateEngineSize(request.EngineSize);
        variant.UpdateModelId(request.ModelId);

        _variantRepository.Update(variant);
        await _variantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
