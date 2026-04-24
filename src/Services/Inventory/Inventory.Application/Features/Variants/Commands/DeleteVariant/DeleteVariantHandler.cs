using Inventory.Application.Exceptions;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using MediatR;

namespace Inventory.Application.Features.Variants.Commands.DeleteVariant;

public class DeleteVariantHandler : IRequestHandler<DeleteVariantCommand, Unit>
{
    private readonly IVariantRepository _variantRepository;

    public DeleteVariantHandler(
        IVariantRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public async Task<Unit> Handle(DeleteVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await _variantRepository.GetByIdAsync(request.Id);

        if (variant is null)
        {
            throw new NotFoundException($"Variant with {request.Id} not found.");
        }

        _variantRepository.Delete(variant);
        await _variantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
